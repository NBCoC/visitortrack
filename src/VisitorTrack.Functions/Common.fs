namespace VisitorTrack.Functions

open System
open VisitorTrack.EntityManager.CustomTypes
open System.Collections.Generic
open System.Net.Http
open Newtonsoft.Json

[<RequireQualifiedAccess>]
module Settings =

    let getStorageOptions () = {
        AccountKey = Environment.GetEnvironmentVariable("DbAccountKey")
        EndpointUrl = Environment.GetEnvironmentVariable("DbEndpointUrl")
        DatabaseId = Environment.GetEnvironmentVariable("DbName")
    }

    let getDefaultPassword () =
        Environment.GetEnvironmentVariable("DefaultPassword")

    let getToken () =
        Environment.GetEnvironmentVariable("Token")

[<AutoOpen>]
module Extensions =

    type HttpRequestMessage with

        member this.TryGetHeaderValue (name : string) =

            if this.Headers.Contains(name) then
                this.Headers.GetValues(name)
                |> Seq.tryHead 
            else None

        member this.TryGetQueryStringValue (key : string) =

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

[<RequireQualifiedAccess>]
module Utility =
    open VisitorTrack.EntityManager.Extensions

    let validateToken (req : HttpRequestMessage) =
        let headerName = "X-Visitor-Track-Token"

        let validate (token : string) =
            if token = Settings.getToken() then
                Result.Ok ()
            else Result.Error "Invalid token"

        req.TryGetHeaderValue headerName
        |> Result.ofOption (sprintf "%s HTTP header is required" headerName)
        |> Result.bind validate

    let getContextUserId (req : HttpRequestMessage) =
        req.TryGetQueryStringValue "contextUserId"
        |> Option.defaultValue String.Empty

    let getEntityId (req : HttpRequestMessage) =
        req.TryGetQueryStringValue "entityId"
        |> Option.defaultValue String.Empty