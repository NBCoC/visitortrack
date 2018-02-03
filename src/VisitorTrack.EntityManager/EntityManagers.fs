namespace VisitorTrack.EntityManager

open System
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open ComputationExpressions
open Entities
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

    let validateEntityId (EntityId entityId) =
        if String.IsNullOrEmpty(entityId) then
            Result.Error "Entity ID is required"
        else (EntityId entityId) |> Result.Ok

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

        let getClient (endpointUrl, accountKey) =
            let url = EndpointUrl.value endpointUrl
            let key = AccountKey.value accountKey
            let uri = Uri(url)

            new DocumentClient(uri, key)

        let! (databaseId, collectionId) = validateCollectionId opts.DatabaseId opts.CollectionId

        let! client =
            validateCredentials opts.EndpointUrl opts.AccountKey
            |> Result.map getClient

        return client, databaseId, collectionId
    }

    let closeConnection (connection, x) =
        let (client: DocumentClient, _, _) = connection

        client.Dispose(); x

    let getCollectionUri databaseId collectionId =
        let database = DatabaseId.value databaseId
        let collection = CollectionId.value collectionId

        UriFactory.CreateDocumentCollectionUri(database, collection)

    let getDocumentUri databaseId collectionId entityId = result {
        let database = DatabaseId.value databaseId
        let collection = CollectionId.value collectionId
        let! (EntityId id) = validateEntityId entityId

        return UriFactory.CreateDocumentUri(database, collection, id)
    }
        

    let createDatabase databaseId (client: DocumentClient) =
        let id = DatabaseId.value databaseId
        let ok _ = client

        client.CreateDatabaseIfNotExistsAsync(Database(Id = id))
        |> taskToResult ok

    let createCollection connection =
        let (client: DocumentClient, databaseId, collectionId) = connection

        let id = DatabaseId.value databaseId
        let col = CollectionId.value collectionId
        let ok _ = connection

        client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(id), DocumentCollection(Id = col))
        |> taskToResult ok

    let createEntity entity connection =
        let (client: DocumentClient, databaseId, collectionId) = connection

        let uri = getCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id |> EntityId

        client.CreateDocumentAsync(uri, entity)
        |> taskToResult ok

    let deleteEntity entityId connection = result {
        let (client: DocumentClient, databaseId, collectionId) = connection

        let! uri = getDocumentUri databaseId collectionId entityId
        let ok _ = ()

        return! client.DeleteDocumentAsync(uri) |> taskToResult ok
    }

[<RequireQualifiedAccess>]
module UserManager =
    open BaseManager

    let canonicalizeRole role =
        match role with
        | 1 -> Admin
        | 2 -> Editor
        | _ -> Viewer

    let getRoleId role =
        match role with
        | Admin -> 1
        | Editor -> 2
        | Viewer -> 3

    let getPasswordValue (HashedPassword x) = x

    let create (opts: StorageOptions) (request: ICreateUser) =
        let (DefaultPassword defaultPassword, dto) = request

        let create = result {
            let! displayName = String75.create "Display Name" dto.DisplayName
            let! emailAddress = EmailAddress.create dto.Email
            let! password = defaultPassword |> HashProvider.hash 
            let role = canonicalizeRole dto.RoleId
            let! connection = getConnection opts

            let entity = {
                id = ""
                token = ""
                displayName = String75.value displayName
                email = EmailAddress.value emailAddress
                password = getPasswordValue password
                roleId = getRoleId role
            }

            let! entityId = createEntity entity connection

            return (connection, entityId)
        }

        create |> Result.map closeConnection

    let delete (opts: StorageOptions) entityId =

        let delete = result {
            let! connection = getConnection opts
            let! data = deleteEntity entityId connection

            return (connection, data)
        }

        delete |> Result.map closeConnection