module Tests

open System
open Xunit
open VisitorTrack.Entities
open VisitorTrack.EntityManager

let [<Literal>] EndpointUri = "https://visitor-track.documents.azure.com:443/"
let [<Literal>] AccountKey = ""

[<Fact>]
let ``Create Demo User`` () =
    use manager = new UserManager("visitor-track-dev", EndpointUri, AccountKey)
    manager.CreateCollectionIfNotExistsAsync() |> Async.AwaitTask |> Async.RunSynchronously

    let user = 
        User(
            Email = "demo.user@nbchurchfamily.org",
            DisplayName = "Demo User",
            Role =  RoleEnum.Editor
        )

    manager.CreateAsync(user) |> Async.AwaitTask |> Async.RunSynchronously

    Assert.True(true)
