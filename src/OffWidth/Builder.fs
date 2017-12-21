namespace OffWidth

[<AutoOpen>]
module Builder =
    type PlanBuilder() =
        member __.Yield () = DataPlan []

        [<CustomOperation("insertInto")>]
        member __.InsertInto (DataPlan items, table: obj) =
            DataPlan [
                yield! items
                yield InsertToTable(table)
            ]
