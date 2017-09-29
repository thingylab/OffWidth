namespace OffWidth

module Postgres =
    module private Internals =
        open System.Data
        open Dapper
        open Npgsql
        open OffWidth.Core.Database

        let infoQuery = """
            select
            	t.schemaname as SchemaName,
            	t.tablename as TableName,
            	c.column_name as ColumnName,
                c.udt_name as ColumnType,
                c.column_default as DefaultValue,
                case when c.is_nullable = 'YES' then true
                	 else false
                end as Nullable,
                c.character_maximum_length as CharacterMaxLength
            from pg_catalog.pg_tables t
            join information_schema.columns c
            	on t.schemaname = c.table_schema
            		and t.tablename = c.table_name
            where t.schemaname not in ('pg_catalog', 'information_schema')"""

        [<CLIMutable>]
        type DbInfo = {
            SchemaName: string
            TableName: string
            ColumnName: string
            ColumnType: string
            DefaultValue: string
            IsNullable: bool
            CharacterMaxLength: int
        }

        let toDbType t =
            match t.ColumnType with
            | "int2" -> Data.Int16
            | "int4" -> Data.Int32
            | "int8" -> Data.Int64
            | "bool" -> Data.Boolean
            | "float4" -> Data.Float
            | "float8" -> Data.Double
            | "numeric" -> Data.Double
            | "money" -> Data.Double
            | "text" -> Data.String t.CharacterMaxLength
            | "date" -> Data.Date
            | "timestamp" -> Data.DateTime
            | "time" -> Data.Time
            | x -> failwith (sprintf "Cannot infer data type from %s" x)

        let toDefinition x =
            { Schema = x.SchemaName
              Table = x.TableName
              Name = x.ColumnName
              DataType = x |> toDbType
              Nullable = x.IsNullable
              Attributes = [] }

        let inspectDatabase connStr =
            use conn = new NpgsqlConnection(connStr)
            conn.Query<DbInfo>(infoQuery)
            |> Seq.map toDefinition

    let inspector connStr =
        Internals.inspectDatabase connStr
