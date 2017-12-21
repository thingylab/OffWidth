namespace OffWidth

[<AutoOpen>]
module Types =
    type PlanStep =
        | InsertToTable of obj

    type DataPlan = DataPlan of PlanStep list
