namespace OffWidth

open System.Reflection
open Chessie.ErrorHandling
open Microsoft.FSharp.Reflection
open RowGenerator
open Session
open Utils

[<AutoOpen>]
module Generate =

    type private GeneratedObjectPropertyDescriptor =
        { PropertyName: string
          InferredColumnName: string
          GetMethod: MethodInfo }

    let private getPropertyDescriptors inflector o =
        let buildDescriptor (p: PropertyInfo) =
            { PropertyName = p.Name
              InferredColumnName = inflector p.Name
              GetMethod = p.GetMethod }

        FSharpType.GetRecordFields (o.GetType())
        |> Array.map buildDescriptor
        |> Array.map (fun x -> x.InferredColumnName, x)
        |> Map.ofArray

    let private objectToMap descriptors o =
        descriptors
        |> Map.map (fun _ d -> d.GetMethod.Invoke(o, Array.empty))

    // TODO: 
    //  - manage non explicitly generated columns with defaults
    //  - Return DbOperations
    //  - Handle relationships
    //  - Handle nullable cols with options
    let generate session table size generator =
        let expectedCols = columns session table
        let firstRow = 
            buildRows generator 1
            |> List.item 0

        trial {
            do! validateType (firstRow.GetType())

            let descriptors = 
                getPropertyDescriptors 
                    Humanizer.InflectorExtensions.Kebaberize
                    firstRow

            let missingColumns = 
                expectedCols
                |> List.map Column.name
                |> List.filter (fun x -> not <| Map.containsKey x descriptors)

            return 
                buildRows generator size
                |> List.map (objectToMap descriptors)
        }

