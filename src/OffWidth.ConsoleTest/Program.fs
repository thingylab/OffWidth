// Descriptive type
type Table<'T> =
    | Table of 'T

type DbType =
    | Integer
    | Float
    | String of int
    | Date
    | DateTime

type Column<'TColumn, 'TTable> =
    | Column of Table<'TTable> * DbType * 'TColumn

type ForeignKey<'TColumn, 'TTable> =
    | ForeignKey of Column<'TColumn, 'TTable> * Column<'TColumn, 'TTable>

type DbDescription<'TColumn, 'TTable> = {
    Columns: Column<'TColumn, 'TTable> list
    ForeignKey: ForeignKey<'TColumn, 'TTable> list
}

// Operation/Action types
type DbOperation<'TTable> =
    | RowInsert of Table<'TTable>
    // At some point: | RowLookup

////////////////////////////////////////////////////////
// Just examples
type MyTables = TableOne | TableTwo
let tableOne, tableTwo = Table TableOne, Table TableTwo
let myDb = {
    Columns =
        [ Column (tableOne, Integer, "id")
          Column (tableOne, String 255, "name")
          Column (tableTwo, Integer, "id")
          Column (tableTwo, Integer, "city") ]
    ForeignKey = []
}
////////////////////////////////////////////////////////



[<EntryPoint>]
let main argv =
    let connStr = "Host=localhost;Username=test;Password=test;Database=test"

    let plan =
        withContext someContext {
            insertInto ""
        }

    0 // return an integer exit code
