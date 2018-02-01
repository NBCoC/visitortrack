#I "./bin/Debug/net461/"
#r "VisitorTrack.Entities"

open System
open VisitorTrack.Entities
open VisitorTrack.Entities.Dtos
open VisitorTrack.Entities.DataTypes

let [<Literal>] EndpointUri = "https://visitor-track.documents.azure.com:443/"
let [<Literal>] AccountKey = "0XSGNIhL2ITzfRi6oCf6nM0nTR8acArm2AASexCEwORmqdUOhKx3TXTGGrphjXbwnxlbnGKCOIqCSVRoFAnn1g=="

let storageOptions = {
    AccountKey = AccountKey
    CollectionId = "UserCollection"
    DatabaseId = "DEV-DB"
    EndpointUrl = EndpointUri
}

let create displayName email role =

    let dto : Dtos.UpsertUserDto = {
        DisplayName = displayName
        Email = email
        RoleId = role
    }

    let command = DefaultPassword "P@55word", dto
    
    UserManager.create storageOptions command |> printfn "%A"

let delete entityId =
    UserManager.delete storageOptions (EntityId entityId) |> printfn "%A"