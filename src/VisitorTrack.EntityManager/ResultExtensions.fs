namespace VisitorTrack.EntityManager

[<RequireQualifiedAccess>]
module Result =

    let either ok error x =
        match x with
        | Ok o -> ok o
        | Error e -> error e

    let ofChoice x =
        match x with
        | Choice1Of2 o -> Ok o
        | Choice2Of2 e -> Error e

    let ofOption e x =
        match x with
        | Some t -> Ok t
        | None -> Error e

    let tee f x = 
        f x; x 

    let tryCatch f handler x =
        try
            f x |> Ok
        with
        | ex -> handler ex |> Error

    let doubleMap ok error =
        either (ok >> Ok) (error >> Error)

    let valueOr f x =
        match x with
        | Ok o -> o
        | Error e -> f e

[<AutoOpen>]
module ResultBuilder =

    type ResultBuilder() =
        member this.Bind(m, f) = Result.bind f m
        
        member this.Return(m) = Ok m

        member this.ReturnFrom(m) = m

        member this.TryFinally(body, compensation) =
            try this.ReturnFrom(body())
            finally compensation() 

        member this.Using(disposable: #System.IDisposable, body) =
            let body' = fun () -> body disposable
            this.TryFinally(body', fun () -> 
                match disposable with 
                    | null -> () 
                    | disp -> disp.Dispose())

    let result = ResultBuilder()