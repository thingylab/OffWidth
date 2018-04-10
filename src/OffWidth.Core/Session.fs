namespace OffWidth

module Session =
    
    let columns session table =
        session.Schema
        |> List.filter (fun (t, _) -> t = table)
        |> List.map snd

