namespace VisitorTrack.EntityManager

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open VisitorTrack.Entities.Models
open VisitorTrack.Entities.Dtos
open DataTypes
open CustomTypes
open VisitorTrack.EntityManager.Extensions

[<RequireQualifiedAccess>]
module EntityManager =

    let getDatabaseUri databaseId =
        UriFactory.CreateDatabaseUri(DatabaseId.value databaseId)

    let getDocumentCollectionUri databaseId collectionId =
        UriFactory.CreateDocumentCollectionUri(DatabaseId.value databaseId, CollectionId.Value collectionId)

    let getDocumentUri databaseId collectionId entityId = 
        result {

            let validateEntityId entityId =
                let (EntityId id) = entityId
                if String.IsNullOrEmpty(id) then
                    Result.Error "Entity ID is required"
                else Result.Ok entityId

            let! (EntityId id) = validateEntityId entityId

            return UriFactory.CreateDocumentUri(DatabaseId.value databaseId, CollectionId.Value collectionId, id)
        }

    let executeTask (opts: StorageOptions) task = 
        result {
            let! endpointUrl = EndpointUrl.create opts.EndpointUrl
            let! accountKey = AccountKey.create opts.AccountKey
            let! databaseId = DatabaseId.create opts.DatabaseId
            let ok _ = ()

            let createDatabase (client: DocumentClient) databaseId =
                let database = Database(Id = DatabaseId.value databaseId)
                
                client.CreateDatabaseIfNotExistsAsync(database) |> Result.fromTask ok

            let createCollection (client: DocumentClient) databaseId =
                let uri = getDatabaseUri databaseId
                let collection = DocumentCollection(Id = CollectionId.Value opts.CollectionId)
                let options = RequestOptions (OfferThroughput = Nullable<int>(2500) )

                client.CreateDocumentCollectionIfNotExistsAsync(uri, collection, options) |> Result.fromTask ok

            let uri = Uri(EndpointUrl.value endpointUrl)
            use client = new DocumentClient(uri, AccountKey.value accountKey)

            do! createDatabase client databaseId
            do! createCollection client databaseId

            return! task client databaseId opts.CollectionId
        }

    let insert entity (client: DocumentClient) databaseId collectionId =
        let uri = getDocumentCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id |> EntityId

        client.CreateDocumentAsync(uri, entity) |> Result.fromTask ok

    let delete (opts: StorageOptions) entityId =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! uri = getDocumentUri databaseId collectionId entityId
                let ok _ = ()

                return! client.DeleteDocumentAsync(uri) |> Result.fromTask ok
            }

        executeTask opts task

    let find<'T> (opts: StorageOptions) entityId =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! uri = getDocumentUri databaseId collectionId entityId
                let ok (response: DocumentResponse<'T>) = response.Document

                return! client.ReadDocumentAsync<'T>(uri) |> Result.fromTask ok
            }

        executeTask opts task

    let replace entityId (entity: #IEntity) (client: DocumentClient) databaseId collectionId = 
        result {
            let! uri = getDocumentUri databaseId collectionId entityId
            let ok _ = ()

            return! client.ReplaceDocumentAsync(uri, entity) |> Result.fromTask ok
        }

    let propertyValueExists (client: DocumentClient) databaseId collectionId propertyName propertyValue = 
        result {
            let uri = getDocumentCollectionUri databaseId collectionId
            let collection = CollectionId.Value collectionId
            let sqlPropertyName = SqlPropertyName.Value propertyName
            let predicate _ = true

            let sql = 
                String.Format("SELECT * FROM {0} WHERE {0}.{1} = '{2}'", 
                    collection, sqlPropertyName, propertyValue)
            
            return
                client.CreateDocumentQuery(uri, sql).ToArray()
                |> Array.tryHead
                |> Option.exists predicate
        }

[<RequireQualifiedAccess>]
module UserManager =

    let canonicalizeRole role =
        match role with
        | UserRoleEnum.Admin | UserRoleEnum.Editor -> role
        | _ -> UserRoleEnum.Viewer

    let validateDefaultPassword (DefaultPassword password) =
        let getValue value = String15.value value |> DefaultPassword

        String15.create DefaultPasswordProperty password
        |> Result.map getValue

    let getHashedPasswordValue (HashedPassword x) = x

    let resetPassword (opts: StorageOptions) entityId defaultPassword =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! (DefaultPassword validPassword) = validateDefaultPassword defaultPassword
                let! hashedPassword = HashProvider.hash validPassword
                let (HashedPassword password) = hashedPassword

                let! entity = EntityManager.find<User> opts entityId
                
                entity.Password <- password

                return! EntityManager.replace entityId entity client databaseId collectionId
            }

        EntityManager.executeTask opts task

    let updatePassword (opts: StorageOptions) entityId (dto: UpdateUserPasswordDto) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! validOldPassword = String15.create OldPasswordProperty dto.OldPassword
                let! validNewPassword = String15.create NewPasswordProperty dto.NewPassword

                let! hashedOldPassword = String15.apply HashProvider.hash validOldPassword
                let! hashedNewPassword = String15.apply HashProvider.hash validNewPassword

                let (HashedPassword oldPsw) = hashedOldPassword
                let (HashedPassword newPsw) = hashedNewPassword

                let! valueExists = EntityManager.propertyValueExists client databaseId collectionId PasswordSqlProperty oldPsw

                if String15.equals validOldPassword validNewPassword then
                    return! Result.Error "Old Password must be different than New Password"
                elif valueExists |> not then
                    return! Result.Error "Old Password is incorrect"
                else
                    let! entity = EntityManager.find<User> opts entityId
                
                    entity.Password <- newPsw

                    return! EntityManager.replace entityId entity client databaseId collectionId
            }

        EntityManager.executeTask opts task

    let authenticate (opts: StorageOptions) (dto: AuthenticateUserDto) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! validEmailAddress = EmailAddress.create dto.EmailAddress
                let! validPassword = String15.create PasswordProperty dto.Password
                let! hashedPassword = String15.apply HashProvider.hash validPassword

                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId
                let emailAddress = EmailAddress.value validEmailAddress
                let password = getHashedPasswordValue hashedPassword
                let collection = CollectionId.Value collectionId
                let emailAddressPropertyName = SqlPropertyName.Value EmailAddressSqlProperty
                let passwordPropertyName = SqlPropertyName.Value PasswordSqlProperty

                let sql = 
                    String.Format("SELECT * FROM {0} WHERE {0}.{1} = '{2}' AND {0}.{3} = '{4}'", 
                        collection, emailAddressPropertyName, emailAddress, passwordPropertyName, password)

                let toDto (user: User) =
                    UserAuthenticatedDto (
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        EmailAddress = user.EmailAddress,
                        RoleId = user.RoleId
                    )

                return! 
                    client.CreateDocumentQuery<User>(uri, sql).ToArray()
                    |> Array.tryHead
                    |> Option.map toDto
                    |> Result.ofOption (sprintf "User with email address of '%s' not found or password is incorrect" emailAddress)
            }

        EntityManager.executeTask opts task

    let getAll (opts: StorageOptions) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId
                return client.CreateDocumentQuery<UserDto>(uri).ToArray()
            }
        
        EntityManager.executeTask opts task

    let update (opts: StorageOptions) entityId (dto: UpsertUserDto) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! validDisplayName = String75.create DisplayNameProperty dto.DisplayName
                let! entity = EntityManager.find<User> opts entityId
                
                entity.DisplayName <- String75.value validDisplayName
                entity.RoleId <- canonicalizeRole dto.RoleId

                return! EntityManager.replace entityId entity client databaseId collectionId
            }
        
        EntityManager.executeTask opts task

    let create (opts: StorageOptions) defaultPassword (dto: UpsertUserDto) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! validEmailAddress = EmailAddress.create dto.EmailAddress
                let emailAddress = EmailAddress.value validEmailAddress
                let! valueExists = EntityManager.propertyValueExists client databaseId collectionId EmailAddressSqlProperty emailAddress

                if valueExists then
                    return! sprintf "User with email address of '%s' already exists" emailAddress |> Result.Error 
                else
                    let! validDisplayName = String75.create DisplayNameProperty dto.DisplayName
                    let! (DefaultPassword password) = validateDefaultPassword defaultPassword
                    let! validPassword = password |> HashProvider.hash

                    let entity = 
                        User (
                            DisplayName = String75.value validDisplayName,
                            EmailAddress = EmailAddress.value validEmailAddress,
                            Password = getHashedPasswordValue validPassword,
                            RoleId = canonicalizeRole dto.RoleId
                        )

                    return! EntityManager.insert entity client databaseId collectionId
            }

        EntityManager.executeTask opts task

    