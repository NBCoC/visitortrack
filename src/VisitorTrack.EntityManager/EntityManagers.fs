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
        let error (ex: exn) =
            ex.ToString()

        Async.AwaitTask task
        |> Async.Catch
        |> Async.RunSynchronously
        |> Result.ofChoice
        |> Result.doubleMap ok error

    let getConnection (opts: StorageOptions) = result {

        let validateCredentials endpointUrl accountKey = result {
            let! url = EndpointUrl.create endpointUrl
            let! key = AccountKey.create accountKey

            return url, key
        }

        let validateCollectionId databaseId collectionId = result {
            let! id = DatabaseId.create databaseId
            let! col = CollectionId.create collectionId

            return id, col
        }

        let createDatabase databaseId (client: DocumentClient) =
            let database = Database(Id = DatabaseId.value databaseId)
            let ok _ = ()

            client.CreateDatabaseIfNotExistsAsync(database) |> taskToResult ok

        let createCollection (client: DocumentClient, databaseId, collectionId) =
            let uri = UriFactory.CreateDatabaseUri(DatabaseId.value databaseId)
            let collection = DocumentCollection(Id = CollectionId.value collectionId)
            let ok _ = ()

            client.CreateDocumentCollectionIfNotExistsAsync(uri, collection) |> taskToResult ok

        let getClient (endpointUrl, accountKey) =
            let url = EndpointUrl.value endpointUrl
            let key = AccountKey.value accountKey
            let uri = Uri(url)

            new DocumentClient(uri, key)

        let! (databaseId, collectionId) = validateCollectionId opts.DatabaseId opts.CollectionId

        let! client =
            validateCredentials opts.EndpointUrl opts.AccountKey
            |> Result.map getClient

        let connection = client, databaseId, collectionId

        do! createDatabase databaseId client
        do! createCollection connection

        return connection
    }

    let closeConnection (connection, x) =
        let (client: DocumentClient, _, _) = connection

        client.Dispose(); x

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
        
    let createEntity entity (client: DocumentClient, databaseId, collectionId) =
        let uri = getCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id |> EntityId

        client.CreateDocumentAsync(uri, entity) |> taskToResult ok

    let deleteEntity entityId (client: DocumentClient, databaseId, collectionId) = result {
        let! uri = getDocumentUri databaseId collectionId entityId
        let ok _ = ()

        return! client.DeleteDocumentAsync(uri) |> taskToResult ok
    }

    let findEntity<'Dto> entityId (client: DocumentClient, databaseId, collectionId) = result {
        let! uri = getDocumentUri databaseId collectionId entityId
        let ok (response: DocumentResponse<'Dto>) = response.Document

        return! client.ReadDocumentAsync<'Dto>(uri) |> taskToResult ok
    }

    let replaceEntity entityId (entity: #IEntity) (client: DocumentClient, databaseId, collectionId) = result {
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

    let getAll (opts: StorageOptions) =

        let get = result {
            let! connection = getConnection opts
            let (client: DocumentClient, databaseId, collectionId) = connection
            let uri = getCollectionUri databaseId collectionId
            let dtos = client.CreateDocumentQuery<UserDto>(uri).ToArray()

            return (connection, dtos)
        }
        
        get |> Result.map closeConnection

    let delete (opts: StorageOptions) entityId =

        let delete = result {
            let! connection = getConnection opts
            let! data = deleteEntity entityId connection

            return (connection, data)
        }

        delete |> Result.map closeConnection

    let find (opts: StorageOptions) entityId =

        let get = result {
            let! connection = getConnection opts
            let! data = findEntity<UserDto> entityId connection

            return (connection, data)
        }

        get |> Result.map closeConnection

    let update (opts: StorageOptions) entityId (dto: UpsertUserDto) =

        let update = result {
            let! displayName = String75.create "Display Name" dto.DisplayName
            let! emailAddress = EmailAddress.create dto.EmailAddress

            let! connection = getConnection opts
            let! entity = findEntity<User> entityId connection
            
            entity.DisplayName <- String75.value displayName
            entity.EmailAddress <- EmailAddress.value emailAddress
            entity.RoleId <- canonicalizeRole dto.RoleId

            let! data = replaceEntity entityId entity connection

            return (connection, data)
        }
        
        update |> Result.map closeConnection

    let create (opts: StorageOptions) defaultPassword (dto: UpsertUserDto) =

        let create = result {
            let! displayName = String75.create "Display Name" dto.DisplayName
            let! emailAddress = EmailAddress.create dto.EmailAddress
            let! (DefaultPassword validPsw) = validateDefaultPassword defaultPassword
            let! password = validPsw |> HashProvider.hash 
            let! connection = getConnection opts

            let entity = 
                User (
                    DisplayName = String75.value displayName,
                    EmailAddress = EmailAddress.value emailAddress,
                    Password = getHashedPasswordValue password,
                    RoleId = canonicalizeRole dto.RoleId
                )

            let! entityId = createEntity entity connection

            return (connection, entityId)
        }

        create |> Result.map closeConnection

    