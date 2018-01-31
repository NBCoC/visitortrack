module Tests

open System
open Xunit
open VisitorTrack.Entities
open VisitorTrack.Database

let [<Literal>] EndpointUri = "https://visitor-track.documents.azure.com:443/"
let [<Literal>] AccountKey = "0XSGNIhL2ITzfRi6oCf6nM0nTR8acArm2AASexCEwORmqdUOhKx3TXTGGrphjXbwnxlbnGKCOIqCSVRoFAnn1g=="

[<Fact>]
let ``My test`` () =
    use manager = new UserManager("visitor-track-dev", EndpointUri, AccountKey)
    manager.CreateCollectionIfNotExistsAsync() |> Async.AwaitTask |> Async.RunSynchronously

    let user = 
        User(
            Email = "adepena@nbchurchfamily.org",
            DisplayName = "Alberto De Pena",
            Role =  RoleEnum.Admin
        )

    manager.CreateAsync(user) |> Async.AwaitTask |> Async.RunSynchronously

    Assert.True(true)
