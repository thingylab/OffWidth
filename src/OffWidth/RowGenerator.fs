namespace OffWidth

open FsCheck

module RowGenerator =
    let buildValues size generator =
        match generator with
        | Function f -> 
            [0..size]
            |> List.map (fun n -> f n)
        | Generator gen ->
            Gen.sample 0 size gen
