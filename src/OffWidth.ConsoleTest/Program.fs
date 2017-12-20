// Learn more about F# at http://fsharp.org

open System
open System.IO
open OffWidth
open OffWidth.Postgres


type DataItem =
    | InsertToTable of obj
type DataPlan = DataPlan of DataItem list

type PlanBuilder() =
    member x.Yield (()) = DataPlan []

    [<CustomOperation("insertInto")>]
    member x.insertInto (DataPlan items, table: obj) =
        DataPlan [
            yield! items
            yield InsertToTable(table)
        ]

    [<CustomOperation("insertIntoWithStuff")>]
    member x.InsertInto (DataPlan items, table: obj, otherStuff: string) =
        DataPlan [
            yield! items
            yield InsertToTable(sprintf "%O%s" obj otherStuff)
        ]

let builder = PlanBuilder()

type Tables =
| Table1

let blah =
    builder {
        insertInto Table1
        insertInto "plop"
        insertIntoWithStuff "3rd" "otherStuff"
    }


[<EntryPoint>]
let main argv =
    let connStr = "Host=localhost;Username=test;Password=test;Database=test"

    let plan =
        withContext someContext {
            insertInto ""
        }

    0 // return an integer exit code
