namespace VisitorTrack.Functions

open System
open System.Net
open System.Net.Http
open Microsoft.Azure.WebJobs.Host
open VisitorTrack.EntityManager
open VisitorTrack.EntityManager.Dtos
open VisitorTrack.EntityManager.DataTypes

module CreateUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing CreateUser func...")

            let defaultPassword = Environment.GetEnvironmentVariable("DefaultPassword")
            let storageOptions = Settings.getStorageOptions "UserCollection"
            let! dto = req.GetDto<UpsertUserDto>()

            let getCommand dto = DefaultPassword defaultPassword, dto
            let ok (EntityId id) = req.CreateResponse(HttpStatusCode.OK, id)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Result.ofOption "User DTO payload is required" dto
                |> Result.map getCommand
                |> Result.bind (UserManager.create storageOptions)
                |> Result.either ok error
        } |> Async.RunSynchronously

module DeleteUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing DeleteUser func...")

            let storageOptions = Settings.getStorageOptions "UserCollection"
            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                req.TryGetQueryStringValue "id" 
                |> Option.map EntityId
                |> Result.ofOption "User ID is required (?id=<userid>)"
                |> Result.bind (UserManager.delete storageOptions)
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module GetUser =

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing GetUser func...")

            let storageOptions = Settings.getStorageOptions "UserCollection"
            let ok dto = req.CreateResponse(HttpStatusCode.OK, dto)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                req.TryGetQueryStringValue "id" 
                |> Option.map EntityId
                |> Result.ofOption "User ID is required (?id=<userid>)"
                |> Result.bind (UserManager.find storageOptions)
                |> Result.either ok error
                    
        } |> Async.RunSynchronously
