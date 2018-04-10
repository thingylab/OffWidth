namespace OffWidth

[<AutoOpen>]
module Types =
    type DbType =
        | Integer
        | Float
        | String of int
        | Date
        | DateTime

    type Table = Table of string

    type Column =
        | Column of string * DbType
        | AutoIncrement of string
        | ForeignKey of string * Table * Column
