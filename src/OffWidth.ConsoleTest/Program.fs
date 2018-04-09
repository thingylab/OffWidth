#load @"C:/dev/OffWidth/src/.paket/load/netstandard2.0/Chessie.fsx"
#load @"C:/dev/OffWidth/src/.paket/load/netstandard2.0/Humanizer.Core.fsx"
#load @"C:/dev/OffWidth/src/.paket/load/netstandard2.0/FsCheck.fsx"


open Microsoft.FSharp.Reflection
open FsCheck
open Chessie.ErrorHandling
open Humanizer
open System
open System.Reflection

// Schema description
type DbType =
    | Integer
    | Float
    | String of int
    | Date
    | DateTime

type Table = Table of string

type Column =
    | Column of string * DbType
    | ForeignKey of string * Table * Column

type NamingStyle =
    | TitleCase
    | PascalCase
    | CamelCase
    | SnakeCase
    | KebabCase

// This will be useful later
type Session =
    { Schema: (Table * Column) list
      ColumnNamingStyle: NamingStyle }

// This should prob be private
type DatabaseOperation<'TColumn>
    when 'TColumn: comparison =
    | InsertRow of Table * Map<'TColumn, obj>

////////////////////////////////////////////////////////
// Examples
type MyTables = TableOne | TableTwo
let mySchema = 
        [ Table "One", Column ("id", Integer)
          Table "One", Column ("first-name", String 255)
          Table "One", Column ("last-name", String 255)
          Table "Two", Column ("id", Integer)
          Table "Two", Column ("city", Integer) ]
////////////////////////////////////////////////////////

type RowGenerator =
    | Function of (int -> obj)
    | Gen of Gen<obj>

type GeneratedObjectPropertyDescriptor =
    { PropertyName: string
      InferredColumnName: string
      GetMethod: MethodInfo }

let getPropertyDescriptors inflector o =
    let buildDescriptor (p: PropertyInfo) =
        { PropertyName = p.Name
          InferredColumnName = inflector p.Name
          GetMethod = p.GetMethod }

    FSharpType.GetRecordFields (o.GetType())
    |> Array.map buildDescriptor
    |> Array.map (fun x -> x.InferredColumnName, x)
    |> Map.ofArray

// TODO: 
//  - manage non explicitly generated columns with defaults
//  - make generator a RowGenerator
//  - use trial
let generate session table size (generator: int -> obj) =
    let expectedCols = 
        session.Schema
        |> List.filter (fun (t, _) -> t = table)
        |> List.map snd

    // Get list of explicitly generated columns
    let firstRow = generator 0
    let generatedType = firstRow.GetType()

    if not <| FSharpType.IsRecord generatedType then failwith "generator does not return a record type"

    let descriptors = 
        getPropertyDescriptors (Humanizer.InflectorExtensions.Kebaberize >> Humanizer.InflectorExtensions.Pluralize) firstRow

    let toName c =
        match c with
        | Column (n, _) -> n
        | ForeignKey (n, _, _) -> n

    let missingColumns = 
        expectedCols
        |> List.map toName
        |> List.filter (fun x -> not <| Map.containsKey x descriptors)

    [0..size]
    |> List.map generator


    // Generate values
    // Return a collection of DatabaseOperations

// Testing
let session = 
    { Schema = mySchema
      ColumnNamingStyle = KebabCase }

type MyRow =
    { FirstName: string
      LastName: string }

let myGenerator n = 
    { FirstName = sprintf "First Name %i" n
      LastName = sprintf "Last Name %i" n } :> obj

generate session (Table "One") 10 myGenerator



type TestType = {NameBlah: string}
let props = FSharpType.GetRecordFields(typeof<TestType>)
let prop = props.[0]
prop.GetMethod.Invoke({NameBlah = "My Name Is Blah"}, Array.empty)


[<EntryPoint>]
let main _ =
    printfn "Hello, world!"

    printfn "%A" (Humanizer.InflectorExtensions.Kebaberize("HelloWorld"))
    
    System.Console.ReadLine()

    0 // return an integer exit code
