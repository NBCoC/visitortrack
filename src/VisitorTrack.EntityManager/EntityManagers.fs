namespace VisitorTrack.EntityManager

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open VisitorTrack.Entities.Models
open VisitorTrack.Entities.Dtos
open DataTypes

[<RequireQualifiedAccess>]
module BaseManager =

    let getCollectionId collectionId =
        match collectionId with
            | UserCollection -> "UserCollection"
            | VisitorCollection -> "VisitorCollection"

    let taskToResult ok task =
        let error (ex: exn) = ex.GetInnerMessage ()

        Async.AwaitTask task
        |> Async.Catch
        |> Async.RunSynchronously
        |> Result.ofChoice
        |> Result.doubleMap ok error

    let getDatabaseUri databaseId =
        UriFactory.CreateDatabaseUri(DatabaseId.value databaseId)

    let getDocumentCollectionUri databaseId collectionId =
        UriFactory.CreateDocumentCollectionUri(DatabaseId.value databaseId, getCollectionId collectionId)

    let getDocumentUri databaseId collectionId entityId = result {

        let validateEntityId entityId =
            let (EntityId id) = entityId
            if String.IsNullOrEmpty(id) then
                Result.Error "Entity ID is required"
            else Result.Ok entityId

        let! (EntityId id) = validateEntityId entityId

        return UriFactory.CreateDocumentUri(DatabaseId.value databaseId, getCollectionId collectionId, id)
    }

    let executeTask (opts: StorageOptions) task = result {
        let! endpointUrl = EndpointUrl.create opts.EndpointUrl
        let! accountKey = AccountKey.create opts.AccountKey
        let! databaseId = DatabaseId.create opts.DatabaseId
        let ok _ = ()

        let createDatabase (client: DocumentClient) databaseId =
            let database = Database(Id = DatabaseId.value databaseId)
            
            client.CreateDatabaseIfNotExistsAsync(database) |> taskToResult ok

        let createCollection (client: DocumentClient) databaseId =
            let uri = getDatabaseUri databaseId
            let collection = DocumentCollection(Id = getCollectionId opts.CollectionId)
            let options = RequestOptions (OfferThroughput = Nullable<int>(2500) )

            client.CreateDocumentCollectionIfNotExistsAsync(uri, collection, options) |> taskToResult ok

        use client = new DocumentClient(Uri(EndpointUrl.value endpointUrl), AccountKey.value accountKey)

        do! createDatabase client databaseId
        do! createCollection client databaseId

        return! task client databaseId opts.CollectionId
    }

    let create entity (client: DocumentClient) databaseId collectionId =
        let uri = getDocumentCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id |> EntityId

        client.CreateDocumentAsync(uri, entity) |> taskToResult ok

    let delete (opts: StorageOptions) entityId =

        let task (client: DocumentClient) databaseId collectionId = result {
            let! uri = getDocumentUri databaseId collectionId entityId
            let ok _ = ()

            return! client.DeleteDocumentAsync(uri) |> taskToResult ok
        }

        executeTask opts task

    let read<'T> (opts: StorageOptions) entityId =

        let task (client: DocumentClient) databaseId collectionId = result {
            let! uri = getDocumentUri databaseId collectionId entityId
            let ok (response: DocumentResponse<'T>) = response.Document

            return! client.ReadDocumentAsync<'T>(uri) |> taskToResult ok
        }

        executeTask opts task

    let replace entityId (entity: #IEntity) (client: DocumentClient) databaseId collectionId = result {
        let! uri = getDocumentUri databaseId collectionId entityId
        let ok _ = ()

        return! client.ReplaceDocumentAsync(uri, entity) |> taskToResult ok
    }

[<RequireQualifiedAccess>]
module UserManager =

    let canonicalizeRole role =
        match role with
        | UserRoleEnum.Admin -> UserRoleEnum.Admin
        | UserRoleEnum.Editor -> UserRoleEnum.Editor
        | _ -> UserRoleEnum.Viewer

    let validateDefaultPassword (DefaultPassword password) =
        let getValue value = String15.value value |> DefaultPassword

        String15.create "Default Password" password
        |> Result.map getValue

    let getHashedPasswordValue (HashedPassword x) = x

    let authenticate (opts: StorageOptions) (dto: AuthenticateUserDto) =

        let task (client: DocumentClient) databaseId collectionId = result {
            let! emailAddress = EmailAddress.create dto.EmailAddress
            let! password = String15.create "Password" dto.Password
            
            let uri = BaseManager.getDocumentCollectionUri databaseId collectionId
            let email = EmailAddress.value emailAddress
            let! hashedPassword = String15.value password |> HashProvider.hash
            let psw = getHashedPasswordValue hashedPassword

            let sql = 
                String.Format("SELECT * FROM {0} WHERE {0}.EmailAddress = '{1}' AND {0}.Password = '{2}'", BaseManager.getCollectionId collectionId, email, psw)

            let toDto (user: User) =
                UserAuthenticatedDto (
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    EmailAddress = user.EmailAddress,
                    RoleId = user.RoleId,
                    Token = user.Token
                )

            return! 
                client.CreateDocumentQuery<User>(uri, sql).ToArray()
                |> Array.tryHead
                |> Option.map toDto
                |> Result.ofOption (sprintf "User with email address of '%s' not found or password is incorrect" email)
        }

        BaseManager.executeTask opts task

    let getAll (opts: StorageOptions) =

        let task (client: DocumentClient) databaseId collectionId = result {
            let uri = BaseManager.getDocumentCollectionUri databaseId collectionId
            return client.CreateDocumentQuery<UserDto>(uri).ToArray()
        }
        
        BaseManager.executeTask opts task

    let update (opts: StorageOptions) entityId (dto: UpsertUserDto) =

        let task (client: DocumentClient) databaseId collectionId = result {
            let! displayName = String75.create "Display Name" dto.DisplayName
            let! entity = BaseManager.read<User> opts entityId
            
            entity.DisplayName <- String75.value displayName
            entity.RoleId <- canonicalizeRole dto.RoleId

            return! BaseManager.replace entityId entity client databaseId collectionId
        }
        
        BaseManager.executeTask opts task

    let create (opts: StorageOptions) defaultPassword (dto: UpsertUserDto) =

        let task (client: DocumentClient) databaseId collectionId = result {
            let! displayName = String75.create "Display Name" dto.DisplayName
            let! emailAddress = EmailAddress.create dto.EmailAddress
            let! (DefaultPassword validPsw) = validateDefaultPassword defaultPassword
            let! password = validPsw |> HashProvider.hash
            let generateToken () = System.Guid.NewGuid().ToString()

            let entity = 
                User (
                    DisplayName = String75.value displayName,
                    EmailAddress = EmailAddress.value emailAddress,
                    Password = getHashedPasswordValue password,
                    RoleId = canonicalizeRole dto.RoleId,
                    Token = generateToken ()
                )

            return! BaseManager.create entity client databaseId collectionId
        }

        BaseManager.executeTask opts task

    