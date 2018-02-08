namespace VisitorTrack.EntityManager

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open VisitorTrack.Entities.Models
open VisitorTrack.Entities.Dtos
open DataTypes

module BaseManager =

    let taskToResult ok task =
        let error (ex: exn) = ex.GetInnerMessage ()

        Async.AwaitTask task
        |> Async.Catch
        |> Async.RunSynchronously
        |> Result.ofChoice
        |> Result.doubleMap ok error

    let getCollectionUri databaseId collectionId =
        let database = DatabaseId.value databaseId
        let collection = CollectionId.value collectionId

        UriFactory.CreateDocumentCollectionUri(database, collection)

    let getDocumentUri databaseId collectionId entityId = result {

        let validateEntityId entityId =
            let (EntityId id) = entityId
            if String.IsNullOrEmpty(id) then
                Result.Error "Entity ID is required"
            else Result.Ok entityId

        let database = DatabaseId.value databaseId
        let collection = CollectionId.value collectionId
        let! (EntityId id) = validateEntityId entityId

        return UriFactory.CreateDocumentUri(database, collection, id)
    }

    let executeTask (opts: StorageOptions) task = result {
        let! endpointUrl = EndpointUrl.create opts.EndpointUrl
        let! accountKey = AccountKey.create opts.AccountKey
        let! databaseId = DatabaseId.create opts.DatabaseId
        let! collectionId = CollectionId.create opts.CollectionId
        let ok _ = ()

        let createDatabase (client: DocumentClient) databaseId =
            let id = DatabaseId.value databaseId
            let database = Database(Id = id)
            
            client.CreateDatabaseIfNotExistsAsync(database) |> taskToResult ok

        let createCollection (client: DocumentClient) databaseId collectionId =
            let db = DatabaseId.value databaseId
            let col = CollectionId.value collectionId
            let uri = UriFactory.CreateDatabaseUri(db)
            let collection = DocumentCollection(Id = col)

            client.CreateDocumentCollectionIfNotExistsAsync(uri, collection) |> taskToResult ok

        let uri = Uri(EndpointUrl.value endpointUrl)
        let key = AccountKey.value accountKey

        use client = new DocumentClient(uri, key)

        do! createDatabase client databaseId
        do! createCollection client databaseId collectionId

        return! task client databaseId collectionId
    }

    let createEntity entity (client: DocumentClient) databaseId collectionId =
        let uri = getCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id |> EntityId

        client.CreateDocumentAsync(uri, entity) |> taskToResult ok

    let deleteEntity entityId (client: DocumentClient) databaseId collectionId = result {
        let! uri = getDocumentUri databaseId collectionId entityId
        let ok _ = ()

        return! client.DeleteDocumentAsync(uri) |> taskToResult ok
    }

    let findEntity<'Dto> entityId (client: DocumentClient) databaseId collectionId = result {
        let! uri = getDocumentUri databaseId collectionId entityId
        let ok (response: DocumentResponse<'Dto>) = response.Document

        return! client.ReadDocumentAsync<'Dto>(uri) |> taskToResult ok
    }

    let replaceEntity entityId (entity: #IEntity) (client: DocumentClient) databaseId collectionId = result {
        let! uri = getDocumentUri databaseId collectionId entityId
        let ok _ = ()

        return! client.ReplaceDocumentAsync(uri, entity) |> taskToResult ok
    }

[<RequireQualifiedAccess>]
module UserManager =
    open BaseManager

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
            
            let uri = getCollectionUri databaseId collectionId
            let email = EmailAddress.value emailAddress
            let! psw = String15.value password |> HashProvider.hash

            let sql = 
                sprintf "SELECT * FROM UserCollection WHERE UserCollection.EmailAddress = '%s' AND UserCollection.Password = '%s'"  
                    email (getHashedPasswordValue psw)

            return! 
                client.CreateDocumentQuery<User>(uri, sql).ToArray()
                |> Array.tryHead
                |> Option.map (fun user -> user.Token)
                |> Result.ofOption (sprintf "User with email address of %s not found or password is incorrect" email)
        }

        executeTask opts task

    let getAll (opts: StorageOptions) =

        let task (client: DocumentClient) databaseId collectionId = result {
            let uri = getCollectionUri databaseId collectionId
            return client.CreateDocumentQuery<UserDto>(uri).ToArray()
        }
        
        executeTask opts task

    let delete (opts: StorageOptions) entityId =

        let task (client: DocumentClient) databaseId collectionId = result {
            return! deleteEntity entityId client databaseId collectionId
        }

        executeTask opts task

    let find (opts: StorageOptions) entityId =

        let task (client: DocumentClient) databaseId collectionId = result {
            return! findEntity<UserDto> entityId client databaseId collectionId
        }

        executeTask opts task

    let update (opts: StorageOptions) entityId (dto: UpsertUserDto) =

        let task (client: DocumentClient) databaseId collectionId = result {
            let! displayName = String75.create "Display Name" dto.DisplayName
            let! emailAddress = EmailAddress.create dto.EmailAddress

            let! entity = findEntity<User> entityId client databaseId collectionId
            
            entity.DisplayName <- String75.value displayName
            entity.EmailAddress <- EmailAddress.value emailAddress
            entity.RoleId <- canonicalizeRole dto.RoleId

            return! replaceEntity entityId entity client databaseId collectionId
        }
        
        executeTask opts task

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

            return! createEntity entity client databaseId collectionId
        }

        executeTask opts task

    