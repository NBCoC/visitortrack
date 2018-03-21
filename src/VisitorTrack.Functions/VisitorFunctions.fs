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

module GetStatusList =

    [<FunctionName("GetStatusListHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing GetStatusList func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        Utility.validateToken req
        |> Result.map VisitorManager.getStatusList
        |> Result.either ok error

module GetAgeGroups =

    [<FunctionName("GetAgeGroupsHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing GetAgeGroups func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        Utility.validateToken req
        |> Result.map VisitorManager.getAgeGroups
        |> Result.either ok error

module DeleteVisitor =

    [<FunctionName("DeleteVisitorHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing DeleteVisitor func...")

        let ok _ = req.CreateResponse(HttpStatusCode.NoContent)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        let toRequest () : DeleteEntityRequest = {
            Options = Settings.getStorageOptions ()
            ContextUserId = Utility.getContextUserId req
            EntityId = Utility.getEntityId req
        }

        let createRequest () =
            toRequest ()
            |> VisitorManager.delete 

        Utility.validateToken req
        |> Result.bind createRequest
        |> Result.either ok error
                    
      