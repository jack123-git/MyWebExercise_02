using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
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
        public string[] CsAttribute {
            get {
                var builder = new List<string>();
                if (IsPrimaryKey) builder.Add("[key]");

                var exportType = DataType switch
                {
                    "char" => Length.ToLower() == "max" ? $"[{DataType}_(5000)]" : $"[{DataType}_({Length})]",
                    "varchar" => Length.ToLower() == "max" ? $"[{DataType}_(5000)]" : $"[{DataType}_({Length})]",
                    "text" => $"[Varchar_(5000)]",
                    "nchar" => Length.ToLower() == "max" ? $"[{DataType}_(5000)]" : $"[{DataType}_({Length})]",
                    "nvarchar" => Length.ToLower() == "max" ? $"[{DataType}_(5000)]" : $"[{DataType}_({Length})]",
                    "ntext" => $"[Nvarchar_(5000)]",
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
                    ////"datetime" => IsNullable ? "DateTime?" : "DateTime",
                    //"varbinary" => IsNullable ? "byte[]?" : "byte[]",
                    _ => ""
                };

                if (exportType != "") builder.Add(exportType);
                //var data = builder.Count switch
                //{
                //    1 => builder[0].ToString(),
                //    2 => builder[0].ToString() + Environment.NewLine + builder[1].ToString(),
                //    _ => ""
                //};
                //return data;
                //var data = builder.ToArray();
                return builder.ToArray();
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
                    "uniqueidentifier" => $"System.Guid",
                    _ => ""
                }; 
            }
        }
        public string  TvpDatatype 
        {
            get {
                return DataType switch
                {
                    "char" => $"[dbo].[udt_String]",
                    "varchar" => $"[dbo].[udt_String]",
                    "text" => $"[dbo].[udt_String]",
                    "nchar" => $"[dbo].[udt_String]",
                    "nvarchar" => $"[dbo].[udt_String]",
                    "ntext" => $"[dbo].[udt_String]",
                    "bit" => $"[dbo].[udt_Bit]",
                    "tinyint" => $"[dbo].[udt_Int]",
                    "smallint" => $"[dbo].[udt_Int]",
                    "int" => $"[dbo].[udt_Int]",
                    "bigint" => $"[dbo].[udt_BigInt]",
                    "decimal" => $"[dbo].[udt_Decimal]",
                    "numeric" => $"[dbo].[udt_Decimal]",
                    "smallmoney" => $"[dbo].[udt_Decimal]",  //不確定
                    "money" => $"[dbo].[udt_Decimal]",  //不確定
                    "real" => $"[dbo].[udt_Decimal]",  //不確定
                    "float" => $"[dbo].[udt_Decimal]",  //不確定
                    "datetime" => $"[dbo].[udt_Datetime]",
                    "datetimeoffset" => $"[dbo].[udt_DatetimeOffset]",
                    "uniqueidentifier" => $"[dbo].[udt_UniqueIdentifier]",
                    //"varbinary" => $"[dbo].[udt_VarBinary]",
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
            set {
                var parts = value.Split('.');
                if (parts.Length == 1)
                {
                    DataType = parts[0].ToLower();
                }
                if (parts.Length == 2)
                {
                    DataType = parts[0].ToLower();
                    Length = parts[1];
                }
                if (parts.Length == 3) 
                {
                    DataType = parts[0].ToLower();
                    NumericPrecision = parts[1];
                    NumericScale = parts[2];
                }
            }
        }

        public string Length { get; set; }

        public string NumericPrecision { get; set; }

        public string NumericScale { get; set; }

        public string Default { get; set; }

        public string Nullable { get; set; }
        public string PKAutoInt { get; set; } // 是否為自動編號欄位 YES/NO

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
        public bool IsAutoint 
        { 
            get
            {
                return PKAutoInt != null
                    && PKAutoInt.IndexOf("YES", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        } // 是否為自動編號欄位

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
