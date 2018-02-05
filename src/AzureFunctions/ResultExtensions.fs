namespace AzureFunctions

[<RequireQualifiedAccess>]
module Result =

    /// Apply Ok or Error function
    let either ok error x =
        match x with
        | Ok o -> ok o
        | Error e -> error e

    /// Convert a choice into a Result<'T, 'TError> function
    let ofChoice x =
        match x with
        | Choice1Of2 o -> Ok o
        | Choice2Of2 e -> Error e

    /// Convert an option into a Result<'T, 'TError> function
    let ofOption e x =
        match x with
        | Some t -> Ok t
        | None -> Error e

    /// Convert a dead-end function into a one-track function
    let tee f x = 
        f x; x 

    /// Convert a one-track function into a switch with exception handler
    let tryCatch f handler x =
        try
            f x |> Ok
        with
        | ex -> handler ex |> Error

    /// Convert two one-track functions into a Result<'T, 'TError> function
    let doubleMap ok error =
        either (ok >> Ok) (error >> Error)

    /// Given a Result<'T, 'TError>, in the Ok case, return the value.
    /// In the Error case, determine the value to return by 
    /// applying a function to the result
    let valueOr f x =
        match x with
        | Ok o -> o
        | Error e -> f e