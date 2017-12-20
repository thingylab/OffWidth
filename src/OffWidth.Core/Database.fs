namespace OffWidth.Core

module Database =
    type ColumnAttribute =
        | PrimaryKey

    [<RequireQualifiedAccess>]
    type Data =
        | String of int
        | Int16
        | Int32
        | Int64
        | Float
        | Double
        | Date
        | Time
        | DateTime
        | Boolean

    type Column =
        { Schema: string
          Table: string
          Name: string
          DataType: Data
          Nullable: bool
          Attributes: ColumnAttribute list }