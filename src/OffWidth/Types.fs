namespace OffWidth

open FsCheck

[<AutoOpen>]
module Types =
    
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
        | Generator of Gen<obj>
