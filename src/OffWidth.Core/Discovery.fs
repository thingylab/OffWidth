namespace OffWidth.Core

open System.Data

module Discovery =
    type Schema =
    | Default
    | Schema of string

    type Table =
    | TableInSchema of Schema * string

    type Column =
    | Column of Table * string

    type Constraint =
    | PrimaryKey
    | Nullable
    | NonNullable

    type Relation =
    | ForeignKey of Column * Column

    type DiscoveryFacility = {
        getTables: Schema -> Table list
        getColumns: Table -> Column list
        getConstraints: Column -> Constraint list
        getType: Column -> DbType
        getTableRelations: Table -> Relation list
        getIncomingRelations: Column -> Relation list
        getOutgoingRelation: Column -> Relation
    }