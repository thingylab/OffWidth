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

let myStrategy =
    { DefaultStrategy.empty 
        with ByColumnName = Map.ofList 
                                [ "first-name", Function (fun n -> sprintf "-> %i" n :> obj) ;
                                  "id", Function (fun n -> n :> obj) ] }

let session = 
    { Schema = mySchema
      ColumnNamingStyle = KebabCase
      DefaultStrategies = myStrategy }

type MyRowForTableOne =
    { FirstName: string
      LastName: string }

let myGenerator n = 
    { FirstName = sprintf "First Name %i" n
      LastName = sprintf "Last Name %i" n } :> obj

[<EntryPoint>]
let main _ =
    let res = ApplyDefaults.applyDefaults myStrategy (List.take 2 mySchema |> List.map snd) 1
    printfn "%A" res

    System.Console.ReadLine() |> ignore

    0
