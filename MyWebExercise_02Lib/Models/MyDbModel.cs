using System;
using System.Collections.Generic;
using System.Text;
using DotLiquid;

namespace JackToolLib.Models
{
    public class MyDbModel : Drop
    {
        public string NameSpace { get; set; }
        //public string DbName { get { return Tables.FirstOrDefault()?.DatabaseName ?? string.Empty; } }
        public List<DbTable> Tables { get; set; }
        public List<string> Usings { get; set; } = new List<string>();

        public Dictionary<string, object> ToTemplateDictionary()
        {
            var dict = new Dictionary<string, object>();

            dict["NameSpace"] = NameSpace;
            //dict["DbName"] = DbName;

            #region using List 輸出 單一using字串
            var usingArray = Usings.ToArray();
            dict["Usings"] = usingArray;
            #endregion

            #region dictTables
            var dictTables = new List<Dictionary<string, object>>();
            if (Tables != null)
            {
                Dictionary<string, object> dictTable;
                foreach (var table in Tables)
                {
                    dictTable = new Dictionary<string, object>();
                    dictTable["DatabaseName"] = table.DatabaseName;
                    dictTable["SchemaName"] = table.SchemaName;
                    dictTable["TableName"] = table.TableName;
                    dictTable["TableFullName"] = table.TableFullName;
                    dictTable["Description"] = table.Description;
                    dictTable["TableType"] = table.TableType;
                    dictTable["IsViewTable"] = table.IsViewTable;

                    var dictColumns = new List<Dictionary<string, object>>();
                    if (table.Columns != null)
                    {
                        Dictionary<string, object> dictColumn;
                        foreach (var column in table.Columns)
                        {
                            dictColumn = new Dictionary<string, object>();
                            dictColumn["ColumnNo"] = column.ColumnNo;
                            dictColumn["DatabaseName"] = column.DatabaseName;
                            dictColumn["SchemaName"] = column.SchemaName;
                            dictColumn["TableName"] = column.TableName;
                            dictColumn["TableFullName"] = column.TableFullName;
                            dictColumn["TableDescription"] = column.TableDescription;
                            dictColumn["TableType"] = column.TableType;
                            dictColumn["ColumnName"] = column.ColumnName;
                            dictColumn["DataType"] = column.DataType;
                            dictColumn["FullDataType"] = column.FullDataType;
                            dictColumn["Length"] = column.Length;
                            dictColumn["NumericPrecision"] = column.NumericPrecision;
                            dictColumn["NumericScale"] = column.NumericScale;
                            dictColumn["Default"] = column.Default;
                            dictColumn["Description"] = column.Description;
                            dictColumn["IsNullable"] = column.IsNullable;
                            dictColumn["IsPrimaryKey"] = column.IsPrimaryKey;
                            dictColumn["IsIdentity"] = column.IsIdentity;
                            dictColumn["IsForeignKey"] = column.IsForeignKey;
                            dictColumns.Add(dictColumn);
                        }
                    }

                    dict["Columns"] = dictColumns;
                    dictTables.Add(dictTable);
                }
            }
            dict["Tables"] = dictTables;
            #endregion

            return dict;
        }
    }
}
