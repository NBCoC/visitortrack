namespace VisitorTrack.Functions

open System
open System.Net
open System.Net.Http
open Microsoft.Azure.WebJobs.Host
open VisitorTrack.Entities
open VisitorTrack.Entities.Dtos
open VisitorTrack.Entities.DataTypes

module CreateUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing CreateUser func...")

            let defaultPassword = Environment.GetEnvironmentVariable("DefaultPassword")
            let storageOptions = Settings.getStorageOptions "UserCollection"
            let! dto = req.GetDto<UpsertUserDto>()

            let getCommand dto =
                DefaultPassword defaultPassword, dto

            let create () =
                dto
                |> Result.ofOption "User DTO payload is required"
                |> Result.map getCommand
                |> Result.bind (UserManager.create storageOptions)

            let result =
                match create () with
                | Error error -> req.CreateResponse(HttpStatusCode.BadRequest, error)
                | Ok (EntityId id) -> req.CreateResponse(HttpStatusCode.OK, id)

            return result
        } |> Async.RunSynchronously

module DeleteUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing DeleteUser func...")

            let storageOptions = Settings.getStorageOptions "UserCollection"

            let delete () = 
                req.TryGetQueryStringValue "id" 
                |> Option.map EntityId
                |> Result.ofOption "User ID is required (?id=<userid>)"
                |> Result.bind (UserManager.delete storageOptions)

            let result =
                match delete () with
                | Error error -> req.CreateResponse(HttpStatusCode.BadRequest, error)
                | Ok _ -> req.CreateResponse(HttpStatusCode.NoContent)

            return result
                    
        } |> Async.RunSynchronously
