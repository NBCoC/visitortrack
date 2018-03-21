namespace VisitorTrack.Functions

open System
open System.Net
open System.Net.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.Azure.WebJobs.Extensions.Http
open VisitorTrack.EntityManager
open VisitorTrack.EntityManager.CustomTypes
open VisitorTrack.EntityManager.Extensions
open VisitorTrack.Entities

module UpdateUser =

    [<FunctionName("UpdateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "put")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing UpdateUser func...")

            let! payload = req.TryGetDto<User>()
            
            let toRequest (model : User) : UpdateUser = {
                UserId = Utility.getEntityId req
                ContextUserId = Utility.getContextUserId req
                Options = Settings.getStorageOptions ()
                Model = model
            }

            let createRequest () =
                Result.ofOption "DTO payload is required" payload
                |> Result.map toRequest

            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Utility.validateToken req
                |> Result.bind createRequest
                |> Result.bind UserManager.update
                |> Result.either ok error
                
        } |> Async.RunSynchronously

module CreateUser =

    [<FunctionName("CreateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing CreateUser func...")

            let! payload = req.TryGetDto<User>()

            let toRequest (model : User) : CreateUser = 
                model.Password <- Settings.getDefaultPassword()
                {
                    Options = Settings.getStorageOptions ()
                    ContextUserId = Utility.getContextUserId req
                    Model = model
                }

            let createRequest () =
                Result.ofOption "DTO payload is required" payload
                |> Result.map toRequest

            let ok entityId = req.CreateResponse(HttpStatusCode.OK, EntityId.value entityId)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Utility.validateToken req
                |> Result.bind createRequest
                |> Result.bind UserManager.create
                |> Result.either ok error

        } |> Async.RunSynchronously

module DeleteUser =

    [<FunctionName("DeleteUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing DeleteUser func...")

            let opts = Settings.getStorageOptions ()
            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            let createUserId () =
                req.TryGetQueryStringValue "id" 
                |> Result.ofOption "User ID is required"
                |> Result.bind EntityId.create

            return 
                Utility.validateToken req
                |> Result.bind createUserId
                |> Result.bind (EntityManager.delete opts CollectionId.User)
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module GetUser =

    [<FunctionName("GetUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing GetUser func...")

            let opts = Settings.getStorageOptions ()
            let ok dto = req.CreateResponse(HttpStatusCode.OK, dto)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                req.TryGetQueryStringValue "id" 
                |> Result.ofOption "User ID is required"
                |> Result.bind EntityId.create
                |> Result.bind (UserManager.find opts)
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module GetAllUsers =

    [<FunctionName("GetAllUsersHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing GetAllUsers func...")

            let opts = Settings.getStorageOptions ()
            let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return 
                UserManager.getAll opts
                |> Result.either ok error
                    
        } |> Async.RunSynchronously

module AuthenticateUser =

    [<FunctionName("AuthenticateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing AuthenticateUser func...")

            let! payload = req.TryGetDto<AuthenticateUser>()

            let toRequest (model: AuthenticateUser) : Authenticate = {
                Options = Settings.getStorageOptions ()
                Model = model
            }

            let ok (model: ReadonlyUser) = 
                let result = 
                    UserAuthenticated (
                        Token = Settings.getToken(),
                        User = model
                    )
                req.CreateResponse(HttpStatusCode.OK, result)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Result.ofOption "DTO payload is required" payload
                |> Result.map toRequest
                |> Result.bind UserManager.authenticate
                |> Result.either ok error

        } |> Async.RunSynchronously

module GetUserRoles =

    [<FunctionName("GetUserRolesHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing GetUserRoles func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)

        UserManager.getRoles ()
        |> ok