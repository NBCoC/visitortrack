namespace VisitorTrack.Functions

open System
open VisitorTrack.EntityManager.DataTypes
open System.Collections.Generic
open System.Net.Http
open Newtonsoft.Json

[<RequireQualifiedAccess>]
module Settings =

    let getStorageOptions collectionId = {
        AccountKey = Environment.GetEnvironmentVariable("DbAccountKey")
        EndpointUrl = Environment.GetEnvironmentVariable("DbEndpointUrl")
        DatabaseId = Environment.GetEnvironmentVariable("DbName")
        CollectionId = collectionId
    }

    let getDefaultPassword () =
        Environment.GetEnvironmentVariable("DefaultPassword")

    let getToken () =
        Environment.GetEnvironmentVariable("Token")

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

        member this.TryGetDto<'Dto> () = 
            async {
                let! payload = 
                    this.Content.ReadAsStringAsync() 
                    |> Async.AwaitTask

                let result =
                    if String.IsNullOrEmpty(payload) then
                        None
                    else JsonConvert.DeserializeObject<'Dto>(payload) |> Some

                return result
            }