open FsCheck
open Chessie.ErrorHandling

// Schema description
type DbType =
    | Integer
    | Float
    | String of int
    | Date
    | DateTime

type Column<'TColumn, 'TTable>
    when 'TColumn: comparison
     and 'TTable: comparison =
    | Column of 'TTable * 'TColumn * DbType
    | ForeignKey of 'TTable * 'TColumn * Column<'TColumn, 'TTable>

type DataGenerationStrategy =
    | Constant of obj
    | Function of (int -> obj)
    | Generator of Gen<obj>

type ForeignKeyGenerationStrategy =
    | CreateRecord
    | FetchRandomly

type DatabaseOperation<'TColumn, 'TTable>
    when 'TColumn: comparison
     and 'TTable: comparison =
    | InsertRow of Map<'TColumn, 'TTable>

////////////////////////////////////////////////////////
// Just examples
type MyTables = TableOne | TableTwo
//let myDb = {
//    Columns =
//        [ Column (TableOne, Integer, "id")
//          Column (TableOne, String 255, "first-name")
//          Column (TableOne, String 255, "last-name")
//          Column (TableTwo, Integer, "id")
//          Column (TableTwo, Integer, "city") ]
//    ForeignKeys = []
//}
////////////////////////////////////////////////////////

let belongsToTable column table =
    match column with
    | Column (t, _, _) -> t = table
    | ForeignKey (t, _, _) -> t = table

let columnsOfTable schema table =
    let separate (dataCols, fks) c =
        match c with
        | Column _ as col -> (col :: dataCols), fks
        | ForeignKey _ as fk -> dataCols, (fk :: fks)

    schema
    |> List.filter (belongsToTable table)
    |> List.fold separate ([], [])

let resolveRelations fks = []

let resolveDataColumns size cols =
    let generateValues size strategy =
        match strategy with
        | Constant v -> List.init size (fun _ -> v)
        | Function f -> List.init size f
        | Generator gen -> Gen.sample 0 size gen

    cols
    |> List.map 

    []

let generate schema size table accept =
    let columns = columnsOfTable schema table

    let acceptedCols, acceptedRelations = accept columns

    resolveRelations acceptedRelations ++ (resolveDataColumns size acceptedCols)


// What I want to be able to write:
let tableOneGenerator =
    let firstNames = ["Pierre"; "Paul"; "Jacques"; "John"; "Bob"]
    gen {
        let! idx = Gen.choose (0, 4)

        return [ "first-name", Value firstNames.[idx]] |> Map.ofList
    }

[<EntryPoint>]
let main _ =
    printfn "Hello, world!"

    let sample = tableOneGenerator |> Gen.sample 0 10

    printfn "%A" sample

    0 // return an integer exit code
