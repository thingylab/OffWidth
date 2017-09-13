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
            IsNullable: string
            CharacterMaxLength: int
        }

        let toDbType t =
            match t with
            | "int2" -> DbType.Int16
            | "int4" -> DbType.Int32
            | "int8" -> DbType.Int64
            | "bool" -> DbType.Boolean
            | "float4" -> DbType.Single
            | "float8" -> DbType.Double
            | "numeric" -> DbType.VarNumeric
            | "money" -> DbType.Currency
            | "text" -> DbType.String
            | "date" -> DbType.Date
            | "timestamp" -> DbType.DateTime
            | "time" -> DbType.Time
            | x -> failwith (sprintf "Cannot infer DbType from %s" x)

        let inspectDatabase connStr =
            let columnInspector info =
                Column (info.ColumnName, toDbType (info.ColumnType), [])

            let inspectColumns infos =
                infos
                |> Seq.map columnInspector
                |> Seq.toList

            let tableMaker (tableName, info) =
                Table (tableName, inspectColumns info)

            let inspectTables schema info =
                info
                |> Seq.groupBy (fun x -> x.TableName)
                |> Seq.map tableMaker
                |> Seq.toList

            let schemaMaker (schemaName, info) =
                Schema (schemaName, inspectTables schemaName info)

            use conn = new NpgsqlConnection(connStr)
            conn.Query<DbInfo>(infoQuery)
            |> Seq.groupBy (fun x -> x.SchemaName)
            |> Seq.map schemaMaker

    let inspector connStr =
        Internals.inspectDatabase connStr
