namespace VisitorTrack.Entities

open System
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open ComputationExpressions
open Models
open DataTypes

module BaseManager =

    let taskToResult ok task =
        let error (ex: exn) =
            ex.ToString()

        task 
        |> Async.AwaitTask
        |> Async.Catch
        |> Async.RunSynchronously
        |> Result.ofChoice
        |> Result.doubleMap ok error

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

    let getDocumentClient (endpointUrl, accountKey) =
        let url = EndpointUrl.value endpointUrl
        let key = AccountKey.value accountKey
        let uri = Uri(url)

        new DocumentClient(uri, key)

    let closeDocumentClient (client: DocumentClient, x) =
        client.Dispose(); x

    let getCollectionUri databaseId collectionId =
        let id = DatabaseId.value databaseId
        let col = CollectionId.value collectionId

        UriFactory.CreateDocumentCollectionUri(id, col)

    let getDocumentUri databaseId collectionId (EntityId entityId) =
        let id = DatabaseId.value databaseId
        let col = CollectionId.value collectionId

        UriFactory.CreateDocumentUri(id, col, entityId)

    let createDatabase databaseId (client: DocumentClient) =
        let id = DatabaseId.value databaseId
        let ok _ = client

        client.CreateDatabaseIfNotExistsAsync(Database(Id = id))
        |> taskToResult ok

    let createCollection databaseId collectionId (client: DocumentClient) =
        let id = DatabaseId.value databaseId
        let col = CollectionId.value collectionId
        let ok _ = client

        client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(id), DocumentCollection(Id = col))
        |> taskToResult ok

    let createEntity databaseId collectionId entity (client: DocumentClient) =
        let uri = getCollectionUri databaseId collectionId
        let ok (response: ResourceResponse<Document>) = response.Resource.Id |> EntityId

        client.CreateDocumentAsync(uri, entity)
        |> taskToResult ok

    let deleteEntity databaseId collectionId entityId (client: DocumentClient) =
        let id = EntityId.value entityId
        let uri = getDocumentUri databaseId collectionId id
        let ok _ = ()

        client.DeleteDocumentAsync(uri)
        |> taskToResult ok

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
        let (defaultPassword, dto) = request

        let create = result {
            let! (databaseId, collectionId) = validateCollectionId opts.DatabaseId opts.CollectionId

            let! displayName = String75.create "Display Name" dto.DisplayName
            let! emailAddress = EmailAddress.create dto.Email
            let! password = DefaultPassword.value defaultPassword |> HashProvider.hash 
            let role = canonicalizeRole dto.RoleId

            let! client =
                validateCredentials opts.EndpointUrl opts.AccountKey
                |> Result.map getDocumentClient

            let entity = {
                Id = ""
                Token = ""
                DisplayName = String75.value displayName
                Email = EmailAddress.value emailAddress
                Password = getPasswordValue password
                RoleId = getRoleId role
            }

            let! entityId = createEntity databaseId collectionId entity client

            return (client, entityId)
        }

        create |> Result.map closeDocumentClient

    let delete (opts: StorageOptions) entityId =

        let delete = result {
            let! (databaseId, collectionId) = validateCollectionId opts.DatabaseId opts.CollectionId

            let! client =
                validateCredentials opts.EndpointUrl opts.AccountKey
                |> Result.map getDocumentClient

            let! data = deleteEntity databaseId collectionId entityId client

            return (client, data)
        }

        delete |> Result.map closeDocumentClient