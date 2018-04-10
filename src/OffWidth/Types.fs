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

    type RowGenerator =
        | Function of (int -> obj)
        | Generator of Gen<obj>
    and DefaultStrategy =
        { ByColumnLogic: (Column -> RowGenerator option) list
          ByColumnName: Map<string, RowGenerator> 
          ByColumnType: Map<DbType, RowGenerator> }
        static member empty = 
            { ByColumnLogic = []; ByColumnName = Map.empty; ByColumnType = Map.empty }

    type Session =
        { Schema: (Table * Column) list
          ColumnNamingStyle: NamingStyle 
          DefaultStrategies: DefaultStrategy }
