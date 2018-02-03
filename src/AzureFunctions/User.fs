namespace AzureFunctions

open System
open System.Configuration
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Microsoft.Azure.WebJobs.Host
open VisitorTrack.EntityManager
open VisitorTrack.EntityManager.Dtos
open VisitorTrack.EntityManager.DataTypes
open System.Collections.Generic

module CreateUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing CreateUser func...")

            let defaultPassword =  ConfigurationManager.AppSettings.["DefaultPassword"]

            let storageOptions = Settings.getStorageOptions "UserCollection"

            let! data = req.Content.ReadAsStringAsync() |> Async.AwaitTask

            if not (String.IsNullOrEmpty(data)) then
                let dto = JsonConvert.DeserializeObject<UpsertUserDto>(data)

                let command = DefaultPassword defaultPassword, dto

                let result =
                    match UserManager.create storageOptions command with
                    | Error message -> req.CreateResponse(HttpStatusCode.BadRequest, message)
                    | Ok (EntityId id) -> req.CreateResponse(HttpStatusCode.OK, id)

                return result
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "User DTO payload is required")
                    
        } |> Async.RunSynchronously

module DeleteUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing DeleteUser func...")

            let storageOptions = Settings.getStorageOptions "UserCollection"

            let idQueryString (item: KeyValuePair<string, string>) =
                if String.Compare(item.Key, "id", true) = 0 then
                    EntityId item.Value |> Some
                else None

            let delete () = 
                req.GetQueryNameValuePairs()
                |> Seq.tryPick idQueryString
                |> Result.ofOption "User ID is required"
                |> Result.bind (UserManager.delete storageOptions)

            let result =
                match delete () with
                | Error message -> req.CreateResponse(HttpStatusCode.BadRequest, message)
                | Ok _ -> req.CreateResponse(HttpStatusCode.NoContent)

            return result
                    
        } |> Async.RunSynchronously
