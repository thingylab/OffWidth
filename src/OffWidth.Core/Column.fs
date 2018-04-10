namespace OffWidth

module Column =
    let name c =
        match c with
        | Column (n, _) -> n
        | ForeignKey (n, _, _) -> n
        | AutoIncrement n -> n

    let dbType c = 
        match c with
        | Column (_, t) -> t
        | _ -> Integer
