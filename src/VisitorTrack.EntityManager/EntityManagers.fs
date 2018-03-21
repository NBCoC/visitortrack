namespace VisitorTrack.EntityManager

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open CustomTypes
open VisitorTrack.EntityManager.Extensions
open VisitorTrack.Entities

[<RequireQualifiedAccess>]
module HashProvider =
    open System.Security.Cryptography
    open System.Text

    let hash str =
        if String.IsNullOrEmpty(str) then
            Result.Error "Value is required for hashing"
        else
            let postSalt = "_buffer_9#00!#8423-12834)*@$920*"
            let preSalt = "visitor_track_salt_"
            let data = Encoding.UTF8.GetBytes(sprintf "%s%s%s" preSalt str postSalt)
            use provider = new SHA256CryptoServiceProvider()
            let hashed = provider.ComputeHash(data)
            
            Convert.ToBase64String(hashed) 
            |> HashedPassword 
            |> Result.Ok

[<RequireQualifiedAccess>]
module EntityManager =

    let internal getDatabaseUri databaseId =
        UriFactory.CreateDatabaseUri(DatabaseId.value databaseId)

    let internal getDocumentCollectionUri databaseId collectionId =
        UriFactory.CreateDocumentCollectionUri(DatabaseId.value databaseId, CollectionId.Value collectionId)

    let internal getDocumentUri databaseId collectionId entityId = 
        UriFactory.CreateDocumentUri(DatabaseId.value databaseId, CollectionId.Value collectionId, EntityId.value entityId)
        
    let internal executeTask (opts: StorageOptions) collectionId task = 
        result {
            let! endpointUrl = EndpointUrl.create opts.EndpointUrl
            let! accountKey = AccountKey.create opts.AccountKey
            let! databaseId = DatabaseId.create opts.DatabaseId
            let ok _ = ()

            let createDatabase (client: DocumentClient) databaseId =
                let database = Database(Id = DatabaseId.value databaseId)
                
                client.CreateDatabaseIfNotExistsAsync(database) 
                |> Result.fromTask ok

            let createCollection (client: DocumentClient) databaseId =
                let uri = getDatabaseUri databaseId
                let collection = DocumentCollection(Id = CollectionId.Value collectionId)
                let options = RequestOptions (OfferThroughput = Nullable<int>(2500) )

                client.CreateDocumentCollectionIfNotExistsAsync(uri, collection, options) 
                |> Result.fromTask ok

            let uri = Uri(EndpointUrl.value endpointUrl)
            use client = new DocumentClient(uri, AccountKey.value accountKey)

            do! createDatabase client databaseId
            do! createCollection client databaseId

            return! task client databaseId collectionId
        }

    let internal insert (client: DocumentClient) databaseId collectionId entity =
        let uri = getDocumentCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id

        client.CreateDocumentAsync(uri, entity) 
        |> Result.fromTask ok
        |> Result.bind EntityId.create

    let delete (opts: StorageOptions) collectionId entityId =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = getDocumentUri databaseId collectionId entityId
                let ok _ = ()

                return! client.DeleteDocumentAsync(uri) |> Result.fromTask ok
            }

        executeTask opts collectionId task

    let internal find<'T> (opts: StorageOptions) collectionId entityId =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = getDocumentUri databaseId collectionId entityId
                let ok (response: DocumentResponse<'T>) = response.Document

                return! client.ReadDocumentAsync<'T>(uri) |> Result.fromTask ok
            }

        executeTask opts collectionId task

    let internal replace (client: DocumentClient) databaseId collectionId entityId entity = 
        result {
            let uri = getDocumentUri databaseId collectionId entityId
            let ok _ = ()

            return! client.ReplaceDocumentAsync(uri, entity) |> Result.fromTask ok
        }

    let internal propertyValueExists (client: DocumentClient) databaseId collectionId (propertyName : string) propertyValue = 
        result {
            let uri = getDocumentCollectionUri databaseId collectionId
            let collection = CollectionId.Value collectionId
            let predicate _ = true

            let sql = 
                String.Format("SELECT * FROM {0} WHERE {0}.{1} = '{2}'", 
                    collection, propertyName, propertyValue)
            
            return
                client.CreateDocumentQuery(uri, sql).ToArray()
                |> Array.tryHead
                |> Option.exists predicate
        }

    let internal getAuthorizedUser (client: DocumentClient) databaseId id =
        result {
            let uri = getDocumentCollectionUri databaseId CollectionId.User

            let sql = 
                ContextUserId.value id 
                |> sprintf "SELECT * FROM UserCollection WHERE UserCollection.id = '%s'"
            
            return!
                client.CreateDocumentQuery<User>(uri, sql).ToArray()
                |> Array.tryHead
                |> Result.ofOption "Context User not found - Unauthorized!"
        }

[<RequireQualifiedAccess>]
module VisitorManager =

    let getAgeGroups () =
        Visitor.AgeGroupLookup()

    let getStatusList () = 
        Visitor.StatusLookup()

[<RequireQualifiedAccess>]
module UserManager =

    let private validateDefaultPassword (DefaultPassword password) =
        let getValue value = 
            Password.value value 
            |> DefaultPassword

        Password.create "Default Password" password
        |> Result.map getValue

    let getRoles () = 
        User.RoleLookup()

    let find opts =
        EntityManager.find<ReadonlyUser> opts CollectionId.User

    let resetPassword (request : ResetPassword) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create request.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId
                
                match authorizedUser.RoleId with
                    | UserRoleEnum.Admin ->
                        let! validPassword = Password.create "Default Password" request.Password
                        let! (HashedPassword password) = Password.apply HashProvider.hash validPassword
                        
                        let! entityId = EntityId.create request.UserId
                        let! entity = EntityManager.find<User> request.Options collectionId entityId
                        
                        entity.Password <- password

                        return! EntityManager.replace client databaseId collectionId entityId entity 
                    | _ -> return! Result.Error "You are not authorized to reset passwords"
            }

        EntityManager.executeTask request.Options CollectionId.User task

    let updatePassword (request : UpdatePassword) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create request.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                let! validOldPassword = Password.create "Old Password" request.Model.OldPassword
                let! validNewPassword = Password.create "New Password" request.Model.NewPassword

                let! (HashedPassword hashedNewPassword) = Password.apply HashProvider.hash validNewPassword

                if Password.equals validOldPassword validNewPassword then
                    return! Result.Error "Old Password must be different than New Password"
                else
                    let! entityId = EntityId.create authorizedUser.Id

                    authorizedUser.Password <- hashedNewPassword
                    
                    return! EntityManager.replace client databaseId collectionId entityId authorizedUser 
            }

        EntityManager.executeTask request.Options CollectionId.User task

    let authenticate (request : Authenticate) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! validEmailAddress = EmailAddress.create request.Model.EmailAddress
                let! validPassword = Password.create "Password" request.Model.Password
                let! (HashedPassword hashedPassword) = Password.apply HashProvider.hash validPassword

                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                let sql = 
                    String.Format(@"SELECT * FROM UserCollection 
                                    WHERE UserCollection.emailAddress = '{0}' 
                                    AND UserCollection.password = '{1}'", 
                         EmailAddress.value validEmailAddress, hashedPassword)

                return! 
                    client.CreateDocumentQuery<ReadonlyUser>(uri, sql).ToArray()
                    |> Array.tryHead
                    |> Result.ofOption (sprintf "User with email address of '%s' not found or password is incorrect" (EmailAddress.value validEmailAddress))
            }

        EntityManager.executeTask request.Options CollectionId.User task

    let getAll (opts: StorageOptions) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                return client.CreateDocumentQuery<ReadonlyUser>(uri).ToArray()
            }
        
        EntityManager.executeTask opts CollectionId.User task

    let update (request : UpdateUser) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create request.ContextUserId
                let! _ = EntityManager.getAuthorizedUser client databaseId contextUserId

                let! validDisplayName = String254.create "Display Name" request.Model.DisplayName

                let! entityId = EntityId.create request.UserId
                let! entity = EntityManager.find<ReadonlyUser> request.Options collectionId entityId

                entity.DisplayName <- String254.value validDisplayName
                entity.RoleId <- request.Model.RoleId
                
                return! EntityManager.replace client databaseId collectionId entityId entity 
            }
        
        EntityManager.executeTask request.Options CollectionId.User task

    let create (request : CreateUser) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create request.ContextUserId
                let! _ = EntityManager.getAuthorizedUser client databaseId contextUserId

                let! validEmailAddress = EmailAddress.create request.Model.EmailAddress
                let emailAddress = EmailAddress.value validEmailAddress

                let! valueExists = EntityManager.propertyValueExists client databaseId collectionId "emailAddress" emailAddress

                if valueExists then
                    return! sprintf "User with email address of '%s' already exists" emailAddress |> Result.Error 
                else
                    let! validDisplayName = String254.create "Display Name" request.Model.DisplayName
                    let! password = Password.create "Password" request.Model.Password
                    let! (HashedPassword validPassword) = Password.apply HashProvider.hash password

                    let user = 
                        User (
                            DisplayName = String254.value validDisplayName,
                            EmailAddress = emailAddress,
                            Password = validPassword,
                            RoleId = request.Model.RoleId
                        )

                    return! EntityManager.insert client databaseId collectionId user 
            }

        EntityManager.executeTask request.Options CollectionId.User task

    