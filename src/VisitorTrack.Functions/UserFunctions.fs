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

module GetUserRoles =

    [<FunctionName("GetUserRolesHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing get user roles func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        Utility.validateToken req
        |> Result.map UserManager.getRoles
        |> Result.either ok error

module UpdateUser =

    [<FunctionName("UpdateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "put")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing update user func...")

            let! payload = req.TryGetDto<User>()
            
            let toRequest (model : User) : UpdateEntity<User> = {
                EntityId = Utility.getEntityId req
                ContextUserId = Utility.getContextUserId req
                Options = Settings.getStorageOptions ()
                Model = model
            }

            let createRequest () =
                Result.ofOption "DTO payload is required" payload
                |> Result.map toRequest
                |> Result.bind UserManager.update

            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Utility.validateToken req
                |> Result.bind createRequest
                |> Result.either ok error
                
        } |> Async.RunSynchronously

module CreateUser =

    [<FunctionName("CreateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing create user func...")

            let! payload = req.TryGetDto<UserAccount>()

            let toRequest (model : UserAccount) : CreateEntity<UserAccount> = 
                model.Password <- Settings.getDefaultPassword()
                {
                    Options = Settings.getStorageOptions ()
                    ContextUserId = Utility.getContextUserId req
                    Model = model
                }

            let createRequest () =
                Result.ofOption "DTO payload is required" payload
                |> Result.map toRequest
                |> Result.bind UserManager.create

            let ok entityId = req.CreateResponse(HttpStatusCode.OK, EntityId.value entityId)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Utility.validateToken req
                |> Result.bind createRequest
                |> Result.either ok error

        } |> Async.RunSynchronously

module DeleteUser =

    [<FunctionName("DeleteUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "delete")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing delete user func...")

        let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        let toRequest () : DeleteEntity = {
            Options = Settings.getStorageOptions ()
            ContextUserId = Utility.getContextUserId req
            EntityId = Utility.getEntityId req
        }

        let createRequest () =
            toRequest ()
            |> UserManager.delete 

        Utility.validateToken req
        |> Result.bind createRequest
        |> Result.either ok error

module GetUser =

    [<FunctionName("GetUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing get user func...")

        let opts = Settings.getStorageOptions ()
        let ok dto = req.CreateResponse(HttpStatusCode.OK, dto)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        let createRequest () =
            Utility.getEntityId req
            |> EntityId.create
            |> Result.bind (UserManager.find opts)

        Utility.validateToken req
        |> Result.bind createRequest
        |> Result.either ok error

module GetAllUsers =

    [<FunctionName("GetAllUsersHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing Get all users func...")

        let opts = Settings.getStorageOptions ()
        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        let createRequest () =
            UserManager.getAll opts

        Utility.validateToken req
        |> Result.bind createRequest
        |> Result.either ok error
                    

module AuthenticateUser =

    [<FunctionName("AuthenticateUserHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing authenticate user func...")

            let! payload = req.TryGetDto<UserAuthentication>()

            let toRequest (model: UserAuthentication) : AuthenticateUser = {
                Options = Settings.getStorageOptions ()
                Model = model
            }

            let ok (model: User) = 
                let result = 
                    UserAuthenticationResult (
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

module UpdateUserPassword =

    [<FunctionName("UpdateUserPasswordHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing update user password func...")

            let! payload = req.TryGetDto<UpdateUserPassword>()

            let toRequest (model: UpdateUserPassword) : UpdatePassword = {
                ContextUserId = Utility.getContextUserId req
                Options = Settings.getStorageOptions ()
                Model = model
            }

            let createRequest () =
                Result.ofOption "DTO payload is required" payload
                |> Result.map toRequest
                |> Result.bind UserManager.updatePassword

            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Utility.validateToken req
                |> Result.bind createRequest
                |> Result.either ok error

        } |> Async.RunSynchronously

module ResetUserPassword =

    [<FunctionName("ResetUserPasswordHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "Executing reset user password func...")

            let toRequest () : ResetPassword = {
                ContextUserId = Utility.getContextUserId req
                UserId = Utility.getEntityId req
                Options = Settings.getStorageOptions ()
                Password = Settings.getDefaultPassword()
            }

            let createRequest () =
                toRequest ()
                |> UserManager.resetPassword

            let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
            let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

            return
                Utility.validateToken req
                |> Result.bind createRequest
                |> Result.either ok error

        } |> Async.RunSynchronously