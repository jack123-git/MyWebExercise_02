using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace JackToolLib.Models
{
    public class DbColumn : Drop
    {
        public int ColumnNo { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string TableFullName { get; set; } // SchemaName.TableName
        public string TableDescription { get; set; }
        public string TableType { get; set; }
        public string ColumnName { get; set; }
        public string CsAttribute {
            get {
                return DataType switch
                {
                    "char" => $"[{DataType}_({Length})]",
                    "varchar" => $"[{DataType}_({Length})]",
                    "text" => $"[Varchar_(max)]",
                    "nchar" => $"[{DataType}_({Length})]",
                    "nvarchar" => $"[{DataType}_({Length})]",
                    "ntext" => $"[Nvarchar_(max)]",
                    //"bit" => IsNullable ? "bool?" : "bool",
                    //"tinyint" => IsNullable ? "byte?" : "byte",
                    //"smallint" => IsNullable ? "short?" : "short",
                    //"int" => IsNullable ? "int?" : "int",
                    //"bigint" => IsNullable ? "long?" : "long",
                    "decimal" => $"[{DataType}_({NumericPrecision},{NumericScale})]",
                    //"numeric" => IsNullable ? "decimal?" : "decimal",
                    //"smallmoney" => IsNullable ? "decimal?" : "decimal",
                    //"money" => IsNullable ? "decimal?" : "decimal",
                    //"real" => IsNullable ? "float?" : "float",
                    //"float" => IsNullable ? "double?" : "double",
                    //"datetime" => IsNullable ? "DateTime?" : "DateTime",
                    //"varbinary" => IsNullable ? "byte[]?" : "byte[]",
                    _ => ""
                };
            }
        }
        public string CsDatatype {
            get 
            { 
                return DataType switch { 
                    "char" => IsNullable?"string?": "string",
                    "varchar" => IsNullable ? "string?" : "string",
                    "text" => IsNullable ? "string?" : "string",
                    "nchar" => IsNullable ? "string?" : "string",
                    "nvarchar" => IsNullable ? "string?" : "string",
                    "ntext" => IsNullable ? "string?" : "string",
                    "bit" => IsNullable ? "bool?" : "bool",                    
                    "tinyint" => IsNullable ? "byte?" : "byte",
                    "smallint" => IsNullable ? "short?" : "short",
                    "int" => IsNullable ? "int?" : "int",
                    "bigint" => IsNullable ? "long?" : "long",
                    "decimal" => IsNullable ? "decimal?" : "decimal",
                    "numeric" => IsNullable ? "decimal?" : "decimal",
                    "smallmoney" => IsNullable ? "decimal?" : "decimal",
                    "money" => IsNullable ? "decimal?" : "decimal",
                    "real" => IsNullable ? "float?" : "float",
                    "float" => IsNullable ? "double?" : "double",
                    "datetime" => IsNullable ? "DateTime?" : "DateTime",
                    "varbinary" => IsNullable ? "byte[]?" : "byte[]",
                    _ => ""
                }; 
            }
        }
        public string DataType { get; set; }
        public string FullDataType
        {
            get
            {
                if (DataType.IndexOf("nText", StringComparison.OrdinalIgnoreCase) >= 0
                    || DataType.IndexOf("image", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return DataType;
                }
                else if (DataType.IndexOf("decimal", StringComparison.OrdinalIgnoreCase) >= 0
                    || DataType.IndexOf("numeric", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return DataType + ( string.Format("({0},{1})", this.NumericPrecision, this.NumericScale));
                }

                return DataType +(string.IsNullOrWhiteSpace(Length) ? "" : string.Format("({0})", Length));
            }
        }

        public string Length { get; set; }

        public string NumericPrecision { get; set; }

        public string NumericScale { get; set; }

        public string Default { get; set; }

        public string Nullable { get; set; }

        public string Identity { get; set; }

        public string Description { get; set; }

        public string PK { get; set; }

        public string PkConstraint { get; set; }

        public string FK { get; set; }

        public string FkConstraint { get; set; }

        public string FkReferencedTable { get; set; }

        public string FkReferencedColumn { get; set; }

        public string FkReferencedInfo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FkReferencedTable) == false)
                {
                    return string.Format("[{0}].[{1}]", FkReferencedTable, FkReferencedColumn);
                }
                return string.Empty;
            }
        }

        public bool IsNullable
        {
            get
            {
                return Nullable != null
                  && Nullable.IndexOf("YES", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public bool IsPrimaryKey
        {
            get
            {
                return PK != null
                  && PK.IndexOf("YES", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public bool IsIdentity
        {
            get
            {
                return Identity != null
                  && Identity.IndexOf("YES", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public bool IsForeignKey
        {
            get
            {
                return FK != null
                  && FK.IndexOf("YES", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
    }
}
