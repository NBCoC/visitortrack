namespace AzureFunctions

open System.Configuration
open VisitorTrack.EntityManager.DataTypes

[<RequireQualifiedAccess>]
module Settings =

    let private accountKey = ConfigurationManager.AppSettings.["DbAccountKey"]
    let private endpointUrl = ConfigurationManager.AppSettings.["DbEndpointUri"]
    let private databaseId = ConfigurationManager.AppSettings.["DbName"]

    let getStorageOptions collectionId = {
        AccountKey = accountKey
        EndpointUrl = endpointUrl
        CollectionId = collectionId
        DatabaseId = databaseId
    }