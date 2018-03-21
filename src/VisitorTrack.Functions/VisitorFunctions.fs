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

        VisitorManager.getStatusList ()
        |> ok

module GetAgeGroups =

    [<FunctionName("GetAgeGroupsHttpTrigger")>]
    let Run([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequestMessage, log: TraceWriter) = 
        log.Info(sprintf "Executing GetAgeGroups func...")

        let ok dtos = req.CreateResponse(HttpStatusCode.OK, dtos)

        VisitorManager.getAgeGroups ()
        |> ok