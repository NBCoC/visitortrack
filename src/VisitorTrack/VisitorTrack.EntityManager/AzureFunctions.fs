namespace VisitorTrack.EntityManager

open System
open System.Net
open System.Net.Http
open Newtonsoft.Json
open Microsoft.Azure.WebJobs.Host
open VisitorTrack.EntityManager.Dtos
open VisitorTrack.EntityManager.DataTypes

module CreateUser =

    let [<Literal>] EndpointUri = "https://visitor-track.documents.azure.com:443/"
    let [<Literal>] AccountKey = "0XSGNIhL2ITzfRi6oCf6nM0nTR8acArm2AASexCEwORmqdUOhKx3TXTGGrphjXbwnxlbnGKCOIqCSVRoFAnn1g=="

    let Run(req: HttpRequestMessage, log: TraceWriter) = 
        async {
            log.Info(sprintf "F# HTTP trigger function processed a request.")

            let storageOptions = {
                AccountKey = AccountKey
                CollectionId = "UserCollection"
                DatabaseId = "DEV-DB"
                EndpointUrl = EndpointUri
            }

            let! data = req.Content.ReadAsStringAsync() |> Async.AwaitTask

            if not (String.IsNullOrEmpty(data)) then
                let dto = JsonConvert.DeserializeObject<UpsertUserDto>(data)

                let command = DefaultPassword "P@55word", dto

                let result =
                    match UserManager.create storageOptions command with
                    | Error message -> req.CreateResponse(HttpStatusCode.BadRequest, message)
                    | Ok (EntityId id) -> req.CreateResponse(HttpStatusCode.OK, "Entity ID " + id)

                return result
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Specify a Name value")
                    
        } |> Async.RunSynchronously

