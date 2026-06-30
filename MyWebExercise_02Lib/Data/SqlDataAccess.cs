using JackToolLib.Models;
using JackToolLib.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection.Emit;


namespace JackToolLib.Data
{
    public interface ISqlDataAccess
    {
        #region User
        //Task<IEnumerable<User>> GetAllUserRolesAsync();
        #endregion
    }

    public class SqlDataAccess(IConfiguration config) : ISqlDataAccess
    {
        private readonly IConfiguration _config = config;
        private readonly string _systemDbName = "TemplateDbConnection";

        #region Database 相關

        /// <summary>
        /// 取得資料庫連線，預設連線到系統資料庫
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private SqlConnection GetDbConnection(string dbName="")
        {
            if (string.IsNullOrEmpty(dbName)) {
                return new SqlConnection(_config.GetConnectionString(_systemDbName, DeCodeType.NONE));
            }
            var currentDbName = new SqlConnectionStringBuilder(_config.GetConnectionString(_systemDbName, DeCodeType.NONE)).InitialCatalog;
            var connection = new SqlConnection(_config.GetConnectionString(_systemDbName, DeCodeType.NONE).Replace($"{currentDbName}", dbName));
            return connection;
        }

        /// <summary>
        /// 取得所有資料庫名稱
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDatabaseNamesAsync()
        {
            string sql = @" Select name as DBName from sys.databases WHERE database_id > 4 ORDER BY name;";
            using var connection = GetDbConnection();
            var result = await connection.QueryAsync<string>(sql);
            connection.Close();
            return result;
        }

        public async Task<List<DbTable>> GetTableSchemaAsync(string dbName)
        {
            using var connection = GetDbConnection(dbName);
            var columns = await connection.QueryAsync<DbColumn>(GetTableSchemaSql());
            var dbTables = columns.GroupBy(p => p.TableFullName,
                               (key, group) => new DbTable()
                               {
                                   TableFullName = key,
                                   DatabaseName = group.Any() ? group.First().DatabaseName : "",
                                   SchemaName = group.Any() ? group.First().SchemaName : "",
                                   TableName = group.Any() ? group.First().TableName : "",
                                   Description = group.Any() ? group.First().TableDescription : "",
                                   TableType = group.Any() ? group.First().TableType : "",
                                   Columns = group.ToList()
                               }
                              ).ToList();

            return dbTables;
        }


        private string GetTableSchemaSql()
        {
            return @"
                SELECT 
                    DB_NAME() AS [DatabaseName],
                    tb.TABLE_SCHEMA as SchemaName,
                    tb.TABLE_NAME as TableName,
                    CASE When tb.TABLE_SCHEMA = 'dbo'
                    THEN
                        tb.TABLE_NAME
                    ELSE 
                        tb.TABLE_SCHEMA + '.' + tb.TABLE_NAME
                    END AS 'TableFullName'
                    ,CASE 
                    WHEN tb.TABLE_TYPE = 'VIEW'
                    THEN (SELECT value 
                            FROM sys.fn_listextendedproperty(NULL, 'user', tb.TABLE_SCHEMA, 'view', tb.TABLE_NAME, DEFAULT, DEFAULT)
                            WHERE name = 'MS_Description'
                            AND objtype = 'VIEW')
                    ELSE (
                        -- 取得 資料表描述
                        SELECT 
                            ep.value
                        FROM sys.objects o
                        INNER JOIN sys.schemas s 
                            ON o.schema_id = s.schema_id
                        LEFT JOIN sys.extended_properties ep 
                            ON o.object_id = ep.major_id
                            AND ep.minor_id = 0 
                            AND ep.name = 'MS_Description'
                        WHERE o.type = 'U' -- U 代表 User Table (使用者資料表)
                            AND o.name = tb.TABLE_NAME 
                            AND s.name = tb.TABLE_SCHEMA

                    )
                    END AS 'TableDescription'
                    ,tb.TABLE_TYPE AS 'TableType'
                    ,col.ORDINAL_POSITION AS 'ColumnNo'
                    ,col.COLUMN_NAME AS 'ColumnName'
                    ,col.DATA_TYPE AS 'DataType'
                    ,CASE 
                    WHEN col.CHARACTER_MAXIMUM_LENGTH = -1
                        THEN 'MAX' 
                        ELSE LTRIM(STR(col.CHARACTER_MAXIMUM_LENGTH,10))
                    END AS 'Length'
                    ,col.NUMERIC_SCALE AS 'NumericScale'
                    ,col.NUMERIC_PRECISION AS 'NumericPrecision'
                    ,col.COLUMN_DEFAULT AS 'Default'
                    ,col.IS_NULLABLE AS 'Nullable'   
                    ,CASE 
                    WHEN COLUMNPROPERTY(object_id(tb.TABLE_NAME), col.COLUMN_NAME, 'IsIdentity') = 1  
                        THEN 'YES' 
                        ELSE 'NO' 
                    END AS 'Identity'  
                    ,(SELECT value
                        FROM sys.fn_listextendedproperty(NULL, 'schema', tb.TABLE_SCHEMA, 'table', tb.TABLE_NAME, 'column', DEFAULT)
                        WHERE name = 'MS_Description'
                        AND objtype = 'COLUMN'
                        AND objname COLLATE Chinese_Taiwan_Stroke_CI_AS = col.COLUMN_NAME) AS 'Description'
                    ,CASE 
                    WHEN tbc.CONSTRAINT_NAME is not null  
                        THEN 'YES' 
                        ELSE 'NO' 
                    END AS 'PK' 
                    ,tbc.CONSTRAINT_NAME AS ' PkConstraint' 
                    ,CASE 
                    WHEN kcu1.CONSTRAINT_NAME is not null  
                        THEN 'YES' 
                        ELSE 'NO' 
                    END AS 'FK'  
                    ,kcu1.CONSTRAINT_NAME AS ' FkConstraint'
                    ,kcu2.TABLE_NAME AS 'FkReferencedTable' 
                    ,kcu2.COLUMN_NAME AS 'FkReferencedColumn'

                FROM 

                INFORMATION_SCHEMA.TABLES tb
                LEFT JOIN INFORMATION_SCHEMA.COLUMNS col ON (tb.TABLE_NAME = col.TABLE_NAME)
                LEFT JOIN
                (
                INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS rc 
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu1 
                    ON kcu1.CONSTRAINT_CATALOG = rc.CONSTRAINT_CATALOG  
                    AND kcu1.CONSTRAINT_SCHEMA = rc.CONSTRAINT_SCHEMA 
                    AND kcu1.CONSTRAINT_NAME = rc.CONSTRAINT_NAME 
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu2 
                    ON kcu2.CONSTRAINT_CATALOG = rc.UNIQUE_CONSTRAINT_CATALOG  
                    AND kcu2.CONSTRAINT_SCHEMA = rc.UNIQUE_CONSTRAINT_SCHEMA 
                    AND kcu2.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME 
                    AND kcu2.ORDINAL_POSITION = kcu1.ORDINAL_POSITION 
                ) ON (tb.TABLE_NAME = kcu1.TABLE_NAME AND col.COLUMN_NAME = kcu1.COLUMN_NAME)
                LEFT JOIN
                (
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tbc
                INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS colc 
                ON  colc.CONSTRAINT_NAME = tbc.CONSTRAINT_NAME
                AND colc.TABLE_NAME = tbc.TABLE_NAME
                AND tbc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                )  ON (tb.TABLE_NAME = tbc.TABLE_NAME AND col.COLUMN_NAME = colc.COLUMN_NAME)
                --where 
                --tb.TABLE_TYPE = 'BASE TABLE'
                --tb.TABLE_NAME = 'SSO_ACTIVE_USER' 
                ORDER BY tb.TABLE_NAME, col.ORDINAL_POSITION   
            ";
        }

        /// <summary>
        /// 取得指定資料庫的所有資料表名稱
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAllTableNamesAsync(string dbName)
        {
            using SqlConnection connection = GetDbConnection(dbName);
            string sql = $@"SELECT [name] as TABLE_NAME FROM sys.tables ORDER BY [name]";
            var result = await connection.QueryAsync<string>(sql);
            connection.Close();
            return result;
        }

        /// <summary>
        /// 檢查資料庫是否存在
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public async Task<bool> DatabaseExistAsync(string dbName)
        {
            string sql = @" Select name as DBName from sys.databases where [name] = @DBName";
            using var connection = GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@DBName", dbName, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<string>(sql, parameters);
            connection.Close();
            return result.ToList().Count > 0;
        }

        /// <summary>
        /// 檢查資料表是否存在
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<bool> TableExistAsync(string dbName, string tableName)
        {
            using SqlConnection connection = GetDbConnection(dbName);

            string sql = $@"SELECT [name] as TABLE_NAME FROM sys.tables WHERE [name] = @TableName";
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<string>(sql, parameters);
            connection.Close();
            return result.ToList().Count > 0;
        }



        #endregion
    }
}
