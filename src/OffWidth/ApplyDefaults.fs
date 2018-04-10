namespace OffWidth

open Chessie.ErrorHandling
open Column

module ApplyDefaults =

    let private findStrategyByColumnLogic column byColumnLogic =
        let firstSome s f =
            match s with
            | Some _ as found -> found
            | None -> f column
        
        byColumnLogic
        |> List.fold firstSome None

    let inline private findStrategyByColumnName column byColumnNameMap =
        Map.tryFind (name column) byColumnNameMap

    let inline private findStrategyByColumnType column byColumnTypeMap =
        Map.tryFind (dbType column) byColumnTypeMap

    let applyDefaults strategy columns size =
        let strategies = 
            columns
            |> List.map (fun c -> 
                c,
                seq {
                    yield findStrategyByColumnLogic c (strategy.ByColumnLogic)
                    yield findStrategyByColumnName c (strategy.ByColumnName)
                    yield findStrategyByColumnType c (strategy.ByColumnType)
                }
                |> Seq.tryFind Option.isSome)

        match List.tryFind (snd >> Option.isNone) strategies with
        | Some (col, _) -> fail <| sprintf "Could not find default strategy for column: %A" col
        | None ->
            strategies
            |> Seq.map (fun (a, b) -> a, (Option.get >> Option.get) b)
            |> Seq.map (fun (col, gen) -> col, RowGenerator.buildValues size gen)
            //|> Seq.collect (fun (col, vals) -> List.map (fun x -> name col, x) vals)
            |> ok
