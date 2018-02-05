namespace AzureFunctions

open System
//open System.Configuration
open AzureFunctions.DataTypes
open System.Collections.Generic
open System.Net.Http
open Newtonsoft.Json

[<RequireQualifiedAccess>]
module Constants =

    let [<Literal>] TokenHeader = "VISITOR-TRACK-TOKEN"


[<RequireQualifiedAccess>]
module Settings =

    let private accountKey = Environment.GetEnvironmentVariable("DbAccountKey")
    let private endpointUrl = Environment.GetEnvironmentVariable("DbEndpointUri")
    let private databaseId = Environment.GetEnvironmentVariable("DbName")

    let getStorageOptions collectionId = {
        AccountKey = accountKey
        EndpointUrl = endpointUrl
        CollectionId = collectionId
        DatabaseId = databaseId
    }

[<AutoOpen>]
module Extensions =

    type HttpRequestMessage with

        member this.TryGetQueryStringValue (key: string) =

            let queryStringValue (item: KeyValuePair<string, string>) =
                if String.Compare(item.Key, key, true) = 0 then
                    Some item.Value
                else None

            this.GetQueryNameValuePairs()
            |> Seq.tryPick queryStringValue

        member this.GetDto<'Dto> () = async {
            let! data = this.Content.ReadAsStringAsync() |> Async.AwaitTask

            let result =
                if String.IsNullOrEmpty(data) then
                    None
                else JsonConvert.DeserializeObject<'Dto>(data) |> Some

            return result
        }
