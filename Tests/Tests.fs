module Tests

open System
open Xunit
open VisitorTrack.EntityManager.Models
open VisitorTrack.EntityManager
open VisitorTrack.EntityManager.Dtos

let [<Literal>] EndpointUri = "https://visitor-track.documents.azure.com:443/"
let [<Literal>] AccountKey = "0XSGNIhL2ITzfRi6oCf6nM0nTR8acArm2AASexCEwORmqdUOhKx3TXTGGrphjXbwnxlbnGKCOIqCSVRoFAnn1g=="

[<Fact>]
let ``Create Demo User`` () =
    use manager = new UserManager("DEV-DB", EndpointUri, AccountKey)

    manager.CreateIfNotExistsAsync() 
    |> Async.AwaitTask 
    |> Async.RunSynchronously

    let dto = 
        UpsertUserDto(
            Email = "demo.user@nbchurchfamily.org",
            DisplayName = "Demo User",
            RoleId =  RoleEnum.Viewer
        )

    let entityId = 
        manager.CreateAsync(dto, "P@55word") 
        |> Async.AwaitTask 
        |> Async.RunSynchronously

    let user =
        manager.Find(entityId)

    let result = 
        manager.AuthenticateAsync(user.Email, "P@55word") 
        |> Async.AwaitTask 
        |> Async.RunSynchronously

    Assert.Equal(user.Email, result.Email);

    manager.DeleteEntityAsync(user.Id) 
    |> Async.AwaitTask 
    |> Async.RunSynchronously

    Assert.True(true);

