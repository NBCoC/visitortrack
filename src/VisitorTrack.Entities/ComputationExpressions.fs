namespace VisitorTrack.Entities

module ComputationExpressions =

    type ResultBuilder() =
        member this.Bind(m, f) = Result.bind f m
        member this.Return(m) = Ok m
        member this.ReturnFrom(m) = m

    let result = ResultBuilder()