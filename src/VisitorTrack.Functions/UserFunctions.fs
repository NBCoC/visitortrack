namespace VisitorTrack.Functions

open System
open System.Net
open System.Net.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.Azure.WebJobs.Extensions.Http
open VisitorTrack.EntityManager
open VisitorTrack.Entities.Dtos

module UpdateUser =

    [<FunctionName("UpdateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Function, "put")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing UpdateUser func...")

            let storageOptions = Settings.getStorageOptions UserCollection
            let! dto = req.GetDto<UpsertUserDto>()
            let entityId =
                req.TryGetQueryStringValue "id" 
                |> Option.defaultValue (String.Empty)
                |> EntityId

            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Result.ofOption "DTO payload is required" dto
                |> Result.bind (UserManager.update storageOptions entityId)
                |> Result.either ok error
        } |> Async.RunSynchronously

module CreateUser =

    [<FunctionName("CreateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Function, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing CreateUser func...")

            let defaultPassword = Environment.GetEnvironmentVariable("DefaultPassword")
            let storageOptions = Settings.getStorageOptions UserCollection
            let! dto = req.GetDto<UpsertUserDto>()

            let password = DefaultPassword defaultPassword
            let ok (EntityId id) = req.CreateResponse(HttpStatusCode.OK, id)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Result.ofOption "DTO payload is required" dto
                |> Result.bind (UserManager.create storageOptions password)
                |> Result.either ok error
        } |> Async.RunSynchronously

module DeleteUser =

    [<FunctionName("DeleteUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Function, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing DeleteUser func...")

            let storageOptions = Settings.getStorageOptions UserCollection
            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                req.TryGetQueryStringValue "id" 
                |> Option.map EntityId
                |> Result.ofOption "User ID is required (?id=<userid>)"
                |> Result.bind (BaseManager.delete storageOptions)
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module GetUser =

    [<FunctionName("GetUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Function, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing GetUser func...")

            let storageOptions = Settings.getStorageOptions UserCollection
            let ok dto = req.CreateResponse(HttpStatusCode.OK, dto)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                req.TryGetQueryStringValue "id" 
                |> Option.map EntityId
                |> Result.ofOption "User ID is required (?id=<userid>)"
                |> Result.bind (BaseManager.find<UserDto> storageOptions)
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module GetAllUsers =

    [<FunctionName("GetAllUsersHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Function, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing GetAllUsers func...")

            let storageOptions = Settings.getStorageOptions UserCollection
            let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                UserManager.getAll storageOptions
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module AuthenticateUser =

    [<FunctionName("AuthenticateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Function, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing AuthenticateUser func...")

            let storageOptions = Settings.getStorageOptions UserCollection
            let! dto = req.GetDto<AuthenticateUserDto>()

            let ok dto = req.CreateResponse(HttpStatusCode.OK, dto)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Result.ofOption "DTO payload is required" dto
                |> Result.bind (UserManager.authenticate storageOptions)
                |> Result.either ok error
        } |> Async.RunSynchronously
