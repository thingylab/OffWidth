namespace OffWidth

open Chessie.ErrorHandling
open Microsoft.FSharp.Reflection

module internal Utils =
    let validateType t =
        if not <| FSharpType.IsRecord t then fail "Generator does not return a record type"
        else ok ()

