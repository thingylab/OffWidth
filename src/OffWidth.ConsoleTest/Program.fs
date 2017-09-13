// Learn more about F# at http://fsharp.org

open System
open System.IO
open OffWidth
open OffWidth.Postgres

[<EntryPoint>]
let main argv =
    let connStr = "Host=localhost;Username=pdoucy;Password=password;Database=pdoucy"
    let res = inspector connStr

    for item in res do
        printfn "%O" item

    0 // return an integer exit code
