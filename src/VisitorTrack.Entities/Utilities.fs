namespace VisitorTrack.Entities

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Security.Cryptography
open System.Text
open System
open DataTypes

[<RequireQualifiedAccess>]
module EntityHelper =

    let serialize entity =
        JsonConvert.SerializeObject(entity, JsonSerializerSettings(ContractResolver = CamelCasePropertyNamesContractResolver()))

module HashProvider =

    let hash str =
        if String.IsNullOrEmpty(str) then
            Error "Value is required for hashing"
        else
            let postSalt = "_buffer_9#00!#8423-12834)*@$920*"
            let preSalt = "visitor_track_salt_"
            let data = Encoding.UTF8.GetBytes(sprintf "%s%s%s" preSalt str postSalt)
            use provider = new SHA256CryptoServiceProvider()
            let hashed = provider.ComputeHash(data)
            Convert.ToBase64String(hashed) |> HashedPassword |> Ok