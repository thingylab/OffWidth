open FsCheck

// Schema description
type DbType =
    | Integer
    | Float
    | String of int
    | Date
    | DateTime

type Column<'TColumn, 'TTable> =
    | Column of 'TTable * 'TColumn * DbType
    | ForeignKey of 'TTable * 'TColumn * Column<'TColumn, 'TTable>

// Generated 'value' from a given column
type ColumnValue =
    | Value of obj
    | FetchRandomly
    | Generate of Gen<obj>

////////////////////////////////////////////////////////
// Just examples
type MyTables = TableOne | TableTwo
let myDb = {
    Columns =
        [ Column (TableOne, Integer, "id")
          Column (TableOne, String 255, "first-name")
          Column (TableOne, String 255, "last-name")
          Column (TableTwo, Integer, "id")
          Column (TableTwo, Integer, "city") ]
    ForeignKeys = []
}
////////////////////////////////////////////////////////

let belongsToTable column table =
    match column with
    | Column (t, _, _) -> t = table
    | ForeignKey (t, _, _) -> t = table

let columnsOfTable schema table =
    schema
    |> List.filter (belongsToTable table)

let generate schema size table accept =
    let columns = columnsOfTable schema table

    [ for i in 1..size do
        yield List.map accept columns ]

// What I want to be able to write:
let tableOneGenerator =
    let firstNames = ["Pierre"; "Paul"; "Jacques"; "John"; "Bob"]
    gen {
        let! idx = Gen.choose (0, 4)

        return [ "first-name", Value firstNames.[idx]] |> Map.ofList
    }


[<EntryPoint>]
let main argv =
    printfn "Hello, world!"

    let sample = tableOneGenerator |> Gen.sample 0 10

    printfn "%A" sample

    0 // return an integer exit code
