namespace VisitorTrack.EntityManager

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open CustomTypes
open VisitorTrack.EntityManager.Extensions
open VisitorTrack.Entities

[<RequireQualifiedAccess>]
module private HashProvider =
    open System.Security.Cryptography
    open System.Text

    let hash str =
        if String.IsNullOrEmpty(str) then
            Result.Error "Cannot hash an empty string"
        else
            let postSalt = "_buffer_9#00!#8423-12834)*@$920*"
            let preSalt = "visitor_track_salt_"
            let data = Encoding.UTF8.GetBytes(sprintf "%s%s%s" preSalt str postSalt)
            use provider = new SHA256CryptoServiceProvider()
            
            provider.ComputeHash(data)
            |> Convert.ToBase64String
            |> HashedPassword 
            |> Result.Ok

[<RequireQualifiedAccess>]
module private EntityManager =

    let getDatabaseUri databaseId =
        UriFactory.CreateDatabaseUri(DatabaseId.value databaseId)

    let getDocumentCollectionUri databaseId collectionId =
        UriFactory.CreateDocumentCollectionUri(DatabaseId.value databaseId, CollectionId.Value collectionId)

    let getDocumentUri databaseId collectionId entityId = 
        UriFactory.CreateDocumentUri(DatabaseId.value databaseId, CollectionId.Value collectionId, EntityId.value entityId)
        
    let executeTask (opts: StorageOptions) collectionId task = 
        result {
            let! endpointUrl = EndpointUrl.create opts.EndpointUrl
            let! accountKey = AccountKey.create opts.AccountKey
            let! databaseId = DatabaseId.create opts.DatabaseId
            let ok _ = ()

            let createDatabaseIfNotExists (client: DocumentClient) databaseId =
                let database = Database(Id = DatabaseId.value databaseId)
                
                client.CreateDatabaseIfNotExistsAsync(database) 
                |> Result.fromTask ok

            let createCollectionIfNotExists (client: DocumentClient) databaseId =
                let uri = getDatabaseUri databaseId
                let collection = DocumentCollection(Id = CollectionId.Value collectionId)
                let options = RequestOptions (OfferThroughput = Nullable<int>(2500))

                client.CreateDocumentCollectionIfNotExistsAsync(uri, collection, options) 
                |> Result.fromTask ok

            let uri = Uri(EndpointUrl.value endpointUrl)
            use client = new DocumentClient(uri, AccountKey.value accountKey)

            do! createDatabaseIfNotExists client databaseId
            do! createCollectionIfNotExists client databaseId

            return! task client databaseId collectionId
        }

    let getAuthorizedUser (client: DocumentClient) databaseId id =
        result {
            let uri = getDocumentCollectionUri databaseId CollectionId.User

            let sql = 
                ContextUserId.value id 
                |> sprintf "SELECT * FROM u WHERE u.id = '%s'"
            
            return!
                client.CreateDocumentQuery<UserAccount>(uri, sql).ToArray()
                |> Array.tryHead
                |> Result.ofOption "Authorized user not found"
        }

    let checkEditorRole (user : User) =
        match user.RoleId with
            | UserRoleEnum.Admin | UserRoleEnum.Editor -> Result.Ok ()
            | _ -> Result.Error "You are not authorized to perform this action" 

    let checkAdminRole (user : User) =
        match user.RoleId with
            | UserRoleEnum.Admin -> Result.Ok ()
            | _ -> Result.Error "You are not authorized to perform this action"

    let insert (client: DocumentClient) databaseId collectionId entity =
        let uri = getDocumentCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id

        client.CreateDocumentAsync(uri, entity) 
        |> Result.fromTask ok
        |> Result.bind EntityId.create

    let delete collectionId (data : DeleteEntity) =

        let task (client: DocumentClient) databaseId collectionId =
            result {
                let! entityId = EntityId.create data.EntityId
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = getAuthorizedUser client databaseId contextUserId
                
                do! checkAdminRole authorizedUser
                
                let uri = getDocumentUri databaseId collectionId entityId
                let ok _ = ()

                return! 
                    client.DeleteDocumentAsync(uri) 
                    |> Result.fromTask ok
            }

        executeTask data.Options collectionId task

    let find<'T> opts collectionId entityId =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = getDocumentUri databaseId collectionId entityId
                let ok (response: DocumentResponse<'T>) = response.Document

                return! 
                    client.ReadDocumentAsync<'T>(uri) 
                    |> Result.fromTask ok
            }

        executeTask opts collectionId task

    let replace (client: DocumentClient) databaseId collectionId entityId entity = 
        result {
            let uri = getDocumentUri databaseId collectionId entityId
            let ok _ = ()

            return! 
                client.ReplaceDocumentAsync(uri, entity) 
                |> Result.fromTask ok
        }

    let hasPropertyValue (client: DocumentClient) databaseId collectionId (propertyName : string) propertyValue = 
        result {
            let uri = getDocumentCollectionUri databaseId collectionId
            let predicate _ = true

            let sql = 
                String.Format("SELECT * FROM a WHERE a.{0} = '{1}'", propertyName, propertyValue)
            
            return
                client.CreateDocumentQuery(uri, sql).ToArray()
                |> Array.tryHead
                |> Option.exists predicate
        }

[<RequireQualifiedAccess>] 
module CheckListManager =

    let get (opts: StorageOptions) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                return client.CreateDocumentQuery<CheckListItem>(uri).ToArray()
            }
        
        EntityManager.executeTask opts CollectionId.CheckList task

[<RequireQualifiedAccess>]
module VisitorManager =

    let private getDescription (model : Visitor) =
        if String.IsNullOrEmpty(model.Description) then
            Ok String.Empty
        else
            String750.create "Description" model.Description
            |> Result.map String750.value

    let private getEmailAddress (model : Visitor) =
        if String.IsNullOrEmpty(model.EmailAddress) then
            Ok String.Empty
        else
            EmailAddress.create model.EmailAddress
            |> Result.map EmailAddress.value

    let private getContactNumber (model : Visitor) =
        if String.IsNullOrEmpty(model.ContactNumber) then
            Ok String.Empty
        else
            ContactNumber.create model.ContactNumber
            |> Result.map ContactNumber.value

    let getAgeGroups () =
        AgeGroups.AsLookup();

    let find opts =
        EntityManager.find<Visitor> opts CollectionId.Visitor

    let delete =
        EntityManager.delete CollectionId.Visitor

    let search (data : CustomTypes.VisitorSearch) =
        
        let task (client : DocumentClient) databaseId collectionId =
            result {
                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                if String.IsNullOrEmpty(data.Text) then
                    return [||]
                else
                    let sql = 
                        String.Format(@"SELECT TOP 25 
                                            v.id, v.fullName, v.ageGroupId, v.isActive, v.firstVisitedOn
                                        FROM v 
                                        WHERE NOT v.hasPlacedMembership
                                        AND v.isActive
                                        AND CONTAINS(v.fullName, '{0}')", 
                            data.Text)

                    return 
                        client.CreateDocumentQuery<VisitorSearch>(uri, sql).ToArray()
            }

        EntityManager.executeTask data.Options CollectionId.Visitor task

    let getReport opts =
        
        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                let today = DateTimeOffset.UtcNow
                let startDate = DateTimeOffset(today.Year, 1, 1, 0, 0, 0, TimeSpan.Zero)

                let memberSql =
                    String.Format(@"SELECT * FROM v
                                    WHERE v.hasPlacedMembership
                                    AND v.becameMemberOn >= '{0}' 
                                    AND v.becameMemberOn <= '{1}'", startDate, today)

                let visitorSql =
                    String.Format(@"SELECT * FROM v
                                    WHERE NOT v.hasPlacedMembership
                                    AND v.isActive
                                    AND v.firstVisitedOn >= '{0}' 
                                    AND v.firstVisitedOn <= '{1}'", startDate, today)

                let memberGroups = 
                    client.CreateDocumentQuery<VisitorLite>(uri, memberSql).ToArray()
                    |> Array.groupBy (fun x -> x.BecameMemberOn.Value.Month)

                let visitorGroups = 
                    client.CreateDocumentQuery<VisitorLite>(uri, visitorSql).ToArray()
                    |> Array.groupBy (fun x -> x.FirstVisitedOn.Value.Month)

                let createReportItem monthId =
                    VisitorReportItem(MonthId = monthId)

                let mapReportItem (dto : VisitorReportItem) =
                    let byMonth (monthId, _) =
                        monthId = dto.MonthId

                    match Array.tryFind byMonth memberGroups with
                        | None -> ()
                        | Some (_, data) -> dto.Members <- data

                    match Array.tryFind byMonth visitorGroups with
                        | None -> ()
                        | Some (_, data) -> dto.Visitors <- data

                    dto

                return [|1..12|] |> Array.map (createReportItem >> mapReportItem)
            }

        EntityManager.executeTask opts CollectionId.Visitor task
        
    let update (data : UpdateEntity<Visitor>) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                do! EntityManager.checkEditorRole authorizedUser

                let! validFullName = String254.create "Full Name" data.Model.FullName
                let! description = getDescription data.Model
                let! emailAddress = getEmailAddress data.Model
                let! contactNumber = getContactNumber data.Model

                let! entityId = EntityId.create data.EntityId
                let! entity = EntityManager.find<Visitor> data.Options collectionId entityId

                entity.FullName <- String254.value validFullName
                entity.Description <- description
                entity.IsActive <- data.Model.IsActive
                entity.EmailAddress <- emailAddress
                entity.ContactNumber <- contactNumber
                entity.AgeGroupId <- data.Model.AgeGroupId
                entity.FirstVisitedOn <- data.Model.FirstVisitedOn
                entity.BecameMemberOn <- data.Model.BecameMemberOn

                return! EntityManager.replace client databaseId collectionId entityId entity 
            }
        
        EntityManager.executeTask data.Options CollectionId.Visitor task

    let create (data : CreateEntity<Visitor>) =

        let getCheckList () = 
            
            let toVisitorCheckListItem (item : CheckListItem) =
                VisitorCheckListItem (
                    Id = item.Id,
                    Type = item.Type,
                    Description = item.Description
                )

            let toVisitorCheckListItems data =
                data
                |> Array.map toVisitorCheckListItem

            CheckListManager.get data.Options
            |> Result.map toVisitorCheckListItems

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                do! EntityManager.checkEditorRole authorizedUser

                let! validFullName = String254.create "Full Name" data.Model.FullName
                let fullName = String254.value validFullName

                let! hasPropertyValue = EntityManager.hasPropertyValue client databaseId collectionId "fullName" fullName

                if hasPropertyValue then
                    return! 
                        sprintf "Visitor with full name of '%s' already exists" fullName 
                        |> Result.Error 
                else
                    let! checkListData = getCheckList ()
                    let! description = getDescription data.Model
                    let! emailAddress = getEmailAddress data.Model
                    let! contactNumber = getContactNumber data.Model

                    let visitor = 
                        Visitor (
                            FullName = fullName,
                            Description = description,
                            AgeGroupId = data.Model.AgeGroupId,
                            EmailAddress = emailAddress,
                            ContactNumber = contactNumber,
                            IsActive = data.Model.IsActive,
                            CreatedOn = DateTimeOffset.UtcNow,
                            FirstVisitedOn = data.Model.FirstVisitedOn,
                            BecameMemberOn = data.Model.BecameMemberOn,
                            CheckList = checkListData
                        )

                    return! EntityManager.insert client databaseId collectionId visitor 
            }

        EntityManager.executeTask data.Options CollectionId.Visitor task

    let updateCheckListItem (data : UpdateVisitorCheckListItem) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                do! EntityManager.checkEditorRole authorizedUser

                let! entityId = EntityId.create data.VisitorId
                let! entity = EntityManager.find<Visitor> data.Options collectionId entityId

                let! validComment = String750.create "Comment" data.Model.Comment
                let! validCompletedBy = String254.create "Completed By" data.Model.CompletedBy

                let! item =
                    entity.CheckList
                    |> Array.tryFind (fun x -> x.Id = data.Model.Id)
                    |> Result.ofOption "Check list item not found"

                let index =
                    Array.IndexOf (entity.CheckList, item)

                let updateItem (oldItem : VisitorCheckListItem) =
                    if oldItem.CompletedOn.HasValue then
                        oldItem.CompletedOn <- Nullable<DateTimeOffset>()
                        oldItem.Comment <- String.Empty
                        oldItem.CompletedBy <- String.Empty
                    else
                        oldItem.CompletedOn <- data.Model.CompletedOn
                        oldItem.Comment <- String750.value validComment
                        oldItem.CompletedBy <- String254.value validCompletedBy
                    oldItem

                entity.CheckList.[index] <- updateItem item

                return! EntityManager.replace client databaseId collectionId entityId entity
            }

        EntityManager.executeTask data.Options CollectionId.Visitor task

[<RequireQualifiedAccess>]
module UserManager =

    let private validateDefaultPassword (DefaultPassword password) =
        let getValue value = 
            Password.value value 
            |> DefaultPassword

        Password.create "Default Password" password
        |> Result.map getValue

    let getRoles () = 
        UserRoles.AsLookup();

    let find opts =
        EntityManager.find<User> opts CollectionId.User

    let delete =
        EntityManager.delete CollectionId.User

    let resetPassword (data : ResetPassword) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId
                
                do! EntityManager.checkAdminRole authorizedUser

                let! validPassword = Password.create "Default Password" data.Password
                let! (HashedPassword password) = Password.apply HashProvider.hash validPassword
                
                let! entityId = EntityId.create data.UserId
                let! entity = EntityManager.find<UserAccount> data.Options collectionId entityId
                
                entity.Password <- password

                return! EntityManager.replace client databaseId collectionId entityId entity 
            }

        EntityManager.executeTask data.Options CollectionId.User task

    let updatePassword (data : UpdatePassword) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                let! validOldPassword = Password.create "Old Password" data.Model.OldPassword
                let! validNewPassword = Password.create "New Password" data.Model.NewPassword

                let! (HashedPassword hashedOldPassword) = Password.apply HashProvider.hash validOldPassword
                let! (HashedPassword hashedNewPassword) = Password.apply HashProvider.hash validNewPassword
                
                if authorizedUser.Password <> hashedOldPassword then
                    return! Result.Error "Old password is not valid"
                elif Password.equals validOldPassword validNewPassword then
                    return! Result.Error "Old password must be different than New password"
                else
                    let! entityId = EntityId.create authorizedUser.Id

                    authorizedUser.Password <- hashedNewPassword
                    
                    return! EntityManager.replace client databaseId collectionId entityId authorizedUser 
            }

        EntityManager.executeTask data.Options CollectionId.User task

    let authenticate (data : AuthenticateUser) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! validEmailAddress = EmailAddress.create data.Model.EmailAddress
                let! validPassword = Password.create "Password" data.Model.Password
                let! (HashedPassword hashedPassword) = Password.apply HashProvider.hash validPassword

                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                let sql = 
                    String.Format(@"SELECT * FROM u 
                                    WHERE u.emailAddress = '{0}' 
                                    AND u.password = '{1}'", 
                         EmailAddress.value validEmailAddress, hashedPassword)

                let error =
                    EmailAddress.value validEmailAddress
                    |> sprintf "User with email address of '%s' not found or password is incorrect"

                return! 
                    client.CreateDocumentQuery<User>(uri, sql).ToArray()
                    |> Array.tryHead
                    |> Result.ofOption error
            }

        EntityManager.executeTask data.Options CollectionId.User task

    let getAll (opts: StorageOptions) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let uri = EntityManager.getDocumentCollectionUri databaseId collectionId

                return client.CreateDocumentQuery<User>(uri).ToArray()
            }
        
        EntityManager.executeTask opts CollectionId.User task

    let update (data : UpdateEntity<User>) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                do! EntityManager.checkAdminRole authorizedUser

                let! validDisplayName = String254.create "Display Name" data.Model.DisplayName

                let! entityId = EntityId.create data.EntityId
                let! entity = EntityManager.find<User> data.Options collectionId entityId

                entity.DisplayName <- String254.value validDisplayName
                entity.RoleId <- data.Model.RoleId
                
                return! EntityManager.replace client databaseId collectionId entityId entity 
            }
        
        EntityManager.executeTask data.Options CollectionId.User task

    let create (data : CreateEntity<UserAccount>) =

        let task (client: DocumentClient) databaseId collectionId = 
            result {
                let! contextUserId = ContextUserId.create data.ContextUserId
                let! authorizedUser = EntityManager.getAuthorizedUser client databaseId contextUserId

                do! EntityManager.checkAdminRole authorizedUser

                let! validEmailAddress = EmailAddress.create data.Model.EmailAddress
                let emailAddress = EmailAddress.value validEmailAddress

                let! hasPropertyValue = EntityManager.hasPropertyValue client databaseId collectionId "emailAddress" emailAddress

                if hasPropertyValue then
                    return! 
                        sprintf "User with email address of '%s' already exists" emailAddress 
                        |> Result.Error 
                else
                    let! validDisplayName = String254.create "Display Name" data.Model.DisplayName
                    let! password = Password.create "Password" data.Model.Password
                    let! (HashedPassword validPassword) = Password.apply HashProvider.hash password

                    let user = 
                        UserAccount (
                            DisplayName = String254.value validDisplayName,
                            EmailAddress = emailAddress,
                            Password = validPassword,
                            RoleId = data.Model.RoleId
                        )

                    return! EntityManager.insert client databaseId collectionId user 
            }

        EntityManager.executeTask data.Options CollectionId.User task

    