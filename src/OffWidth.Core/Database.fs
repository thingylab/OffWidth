namespace OffWidth.Core

module Database =
    open System.Data

    type ColumnAttributes =
        | Nullable
        | NotNullable
    type Column =
        | Column of string * DbType * (ColumnAttributes list)
        | ReferenceColumn of Column * Column
    type Table = Table of string * (Column list)
    type Schema = Schema of string * (Table list)
