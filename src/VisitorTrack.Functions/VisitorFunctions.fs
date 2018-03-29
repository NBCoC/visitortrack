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
        log.Info(sprintf "Executing get status list func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        Utility.validateToken req
        |> Result.map VisitorManager.getStatusList
        |> Result.either ok error

module GetAgeGroups =

    [<FunctionName("GetAgeGroupsHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing get age groups func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        Utility.validateToken req
        |> Result.map VisitorManager.getAgeGroups
        |> Result.either ok error

module GetVisitor =

    [<FunctionName("GetVisitorHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing get visitor func...")

        let opts = Settings.getStorageOptions ()
        let ok dto = req.CreateResponse(HttpStatusCode.OK, dto)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        let createRequest () =
            Utility.getEntityId req
            |> EntityId.create
            |> Result.bind (VisitorManager.find opts)

        Utility.validateToken req
        |> Result.bind createRequest
        |> Result.either ok error

module DeleteVisitor =

    [<FunctionName("DeleteVisitorHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "delete")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing delete visitor func...")

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
                    
module SearchVisitors =

    [<FunctionName("SearchVisitorsHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing search visitors func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)
        let error message = req.CreateResponse(HttpStatusCode.BadRequest, message)

        let toRequest () : VisitorSearchRequest = {
            Options = Settings.getStorageOptions ()
            Text = Utility.getVisitorSearchCriteria req
        }

        let createRequest () =
            toRequest ()
            |> VisitorManager.search 

        Utility.validateToken req
        |> Result.bind createRequest
        |> Result.either ok error