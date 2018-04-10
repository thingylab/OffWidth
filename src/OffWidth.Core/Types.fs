namespace OffWidth

open FsCheck

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

    type NamingStyle =
        | TitleCase
        | PascalCase
        | CamelCase
        | SnakeCase
        | KebabCase

    type Session =
        { Schema: (Table * Column) list
          ColumnNamingStyle: NamingStyle }

    type RowGenerator =
        | Function of (int -> obj)
        | Gen of Gen<obj>
