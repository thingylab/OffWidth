open Microsoft.FSharp.Reflection
open FsCheck
open Chessie.ErrorHandling
open Humanizer
open System
open System.Reflection
open OffWidth

type DatabaseOperation =
    | InsertRow of Table * Map<Column, obj>

type RelationshipResolutionStrategy =
    | CreateRecord
    | UseRandomRecord

////////////////////////////////////////////////////////
// Testing
let mySchema = 
        [ Table "One", Column ("id", Integer)
          Table "One", Column ("first-name", String 255)
          Table "One", Column ("last-name", String 255)
          Table "Two", Column ("id", Integer)
          Table "Two", Column ("city", Integer) ]

let session = 
    { Schema = mySchema
      ColumnNamingStyle = KebabCase }

type MyRowForTableOne =
    { FirstName: string
      LastName: string }

let myGenerator n = 
    { FirstName = sprintf "First Name %i" n
      LastName = sprintf "Last Name %i" n } :> obj

[<EntryPoint>]
let main _ =
    let stuff = generate session (Table "One") 5 (Function myGenerator)
    printfn "%A" stuff

    System.Console.ReadLine() |> ignore

    0
