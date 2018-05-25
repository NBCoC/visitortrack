
open System.Security.Cryptography
open System.Text
open System

module HashProvider =

    let hash str =
        if String.IsNullOrEmpty(str) then
            Result.Error "Cannot hash an empty string"
        else
            let postSalt = "_buffer_9#00!#8423-12834)*@$920*"
            let preSalt = "visitor_track_salt_"
            let data = Encoding.UTF8.GetBytes(sprintf "%s%s%s" preSalt str postSalt)
            use provider = new SHA256CryptoServiceProvider()
            
            provider.ComputeHash(data)
            |> Convert.ToBase64String 
            |> Ok