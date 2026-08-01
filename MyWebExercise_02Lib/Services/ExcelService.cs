using JackToolLib.Extension;
using JackToolLib.Interfaces;
using JackToolLib.Models;
using MathNet.Numerics.Optimization;
using MudBlazor;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace JackToolLib.Services
{
    public class ExcelService : IExcelService
    {
        // IProgress<(int current, int total)> progress 用來回傳匯出進度
        public async Task<byte[]> GenerateDocumentAsync(List<DbTable> DbTables, List<string> tableNames, string templatePath, IProgress<(int current, int total)> progress)
        {
            const string tagName4DatabaseName = "#databasename";
            const string tagName4TableSchemaName = "#table.schemaname";
            const string tagName4TableFullName = "#table.fullname";

            const string sheetName4TableListTempalte = "#TableListTemplate";
            const string sheetName4TableSchemaTempalte = "#TableSchemaTemplate";
            const string tagName4TableNo = "#table.no";
            const string tagName4TableName = "#table.name";
            const string tagName4TableDesc = "#table.description";
            const string tagName4TableView = "#table.view";
            const string tagName4TableType = "#table.type";
            const string tagName4ColumnNo = "#column.no";
            const string tagName4ColumnName = "#column.name";
            const string tagName4ColumnPK = "#column.pk";
            const string tagName4ColumnAutoInt = "#column.autoint";
            const string tagName4ColumnFK = "#column.fk";
            const string tagName4ColumnFKReference = "#column.fkreference";
            const string tagName4ColumnNullable = "#column.nullable";
            const string tagName4ColumnIdentity = "#column.identity";
            const string tagName4ColumnDataType = "#column.datatype";
            const string tagName4ColumnDefault = "#column.default";
            const string tagName4ColumnDesc = "#column.description";


            // open template file
            XSSFWorkbook workbook;
            using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);
            }

            // TABLE LIST
            // clone tempalte table list sheet & rename sheet
            var tableSheet = workbook.CloneSheet(workbook.GetSheetIndex(sheetName4TableListTempalte));
            workbook.SetSheetName(workbook.NumberOfSheets - 1, "Table List");

            // DatabaseName
            tableSheet.SetFirstMatchCellContent(tagName4DatabaseName, DbTables.FirstOrDefault().DatabaseName);

            var tableIndex = 1;
            foreach (var table in DbTables.Where(t => tableNames.Contains(t.TableName)))
            {
                var noLocation = tableSheet.FindCellLocation(tagName4TableNo);
                if (noLocation.HasValue)
                {
                    // copy the table info row from tempalte
                    tableSheet.CopyRow(noLocation.Value.Y, noLocation.Value.Y + 1);

                    var newRowIndex = noLocation.Value.Y;
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableNo, tableIndex.ToString());
                    tableSheet.SetFirstMatchCellHyperlinkInRow(newRowIndex, tagName4TableSchemaName, table.SchemaName, tableIndex.ToString());
                    tableSheet.SetFirstMatchCellHyperlinkInRow(newRowIndex, tagName4TableName, table.TableName, tableIndex.ToString());
                    tableSheet.SetFirstMatchCellHyperlinkInRow(newRowIndex, tagName4TableFullName, table.TableFullName, tableIndex.ToString());
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableDesc, table.Description);
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableView, table.IsViewTable ? "V" : "");
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableType, table.TableType.ToTitleCase());

                    tableIndex++;
                }

            }

            // remove the last template row
            tableSheet.RemoveFirstMatchRow(tagName4TableNo);


            // TABLE SCHEMA SHEETS
            int total = tableNames.Count;
            int current = 0;

            tableIndex = 1;
            foreach (var table in DbTables.Where(t => tableNames.Contains(t.TableName)))
            {
                // clone tempalte schema sheet & rename sheet
                var schemaSheet = workbook.CloneSheet(workbook.GetSheetIndex(sheetName4TableSchemaTempalte));
                workbook.SetSheetName(workbook.NumberOfSheets - 1, tableIndex.ToString()/*table.TableName*/);

                // table name & description
                schemaSheet.SetFirstMatchCellContent(tagName4TableSchemaName, table.SchemaName);
                schemaSheet.SetFirstMatchCellContent(tagName4TableName, table.TableName);
                schemaSheet.SetFirstMatchCellContent(tagName4TableFullName, table.TableFullName);
                schemaSheet.SetFirstMatchCellContent(tagName4TableDesc, table.Description);
                schemaSheet.SetFirstMatchCellContent(tagName4TableView, table.IsViewTable ? "V" : "");
                schemaSheet.SetFirstMatchCellContent(tagName4TableType, table.TableType.ToTitleCase());

                // prepare each column info
                foreach (var column in table.Columns)
                {
                    var noLocation = schemaSheet.FindCellLocation(tagName4ColumnNo);
                    if (noLocation.HasValue)
                    {
                        // copy the column info row from tempalte
                        schemaSheet.CopyRow(noLocation.Value.Y, noLocation.Value.Y + 1);

                        var newRowIndex = noLocation.Value.Y;
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnNo, column.ColumnNo.ToString());
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnName, column.ColumnName);
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnPK, column.IsPrimaryKey ? "V" : "");
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnAutoInt, column.IsAutoint ? "V" : "");
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnFK, column.IsForeignKey ? "V" : "");
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnFKReference, column.FkReferencedInfo);
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnNullable, column.IsNullable ? "V" : "");
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnIdentity, column.IsIdentity ? "V" : "");
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnDataType, column.FullDataType);
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnDefault, column.Default);
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnDesc, column.Description);

                    }

                }

                // remove the last template row
                schemaSheet.RemoveFirstMatchRow(tagName4ColumnNo);

                await Task.Delay(75);
                current++;
                tableIndex++;
                progress?.Report((current, total));
            }

            // remove template sheet
            workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName4TableListTempalte));
            workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName4TableSchemaTempalte));

            //var outputDocPath = $@"D:\schema.xlsx";

            // save as another excel file
            //using (FileStream stream = new FileStream(outputDocPath, FileMode.Create, FileAccess.Write))
            //{
            //    workbook.Write(stream);
            //}

            // save to memory stream and return byte array
            using var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);
            // set null 
            workbook = null;

            return memoryStream.ToArray();
        }

        public async IAsyncEnumerable<(DbTable table, int current, int total)> ImportExcelTableSchemaAsync(Stream fileStream, string fileName)
        {
            IWorkbook workbook;
            if (Path.GetExtension(fileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                workbook = new HSSFWorkbook(fileStream);
            else if (Path.GetExtension(fileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                workbook = new XSSFWorkbook(fileStream);
            else
                throw new NotSupportedException("未支援的檔案格式");


            // 讀取TableSheet
            #region 讀取資料庫名稱
            List<DbTable> tables = new List<DbTable>();
            var dbName = string.Empty;
            var tablesheetIndex = -1;
            for (int i = 0; i <= workbook.NumberOfSheets - 1; i++)
            {
                tablesheetIndex = i;
                if (IsTableSheet(workbook.GetSheetAt(i)))
                {
                    var sheet = workbook.GetSheetAt(i);
                    for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var row = sheet.GetRow(rowIndex);
                        var item = row.GetCell(0).StringCellValue;
                        if (item.StartsWith("資料庫名稱"))
                        {
                            dbName = row.GetCell(2).StringCellValue;
                        }

                        //if (int.TryParse(item.Substring(1), out tablesheet))
                        //{
                        //    var tableFullName = row.GetCell(2).StringCellValue;
                        //    var tableDesc = row.GetCell(4).StringCellValue;
                        //    var (tableSchema, tableName) = SplitTableFullName(tableFullName);
                        //    var table = new DbTable
                        //    {
                        //        DatabaseName = dbName,
                        //        SchemaName = tableSchema,
                        //        TableName = tableName,
                        //        Description = tableDesc,
                        //        Columns = new List<DbColumn>()
                        //    };
                        //    tables.Add(table);
                        //}
                    }
                    break; // 找到TableSheet後就退出迴圈
                }
            }
            #endregion

            int total = workbook.NumberOfSheets; // 總行數 (不包含標題列)
            int current = 1; // 當前Table
            Dictionary<string, int> columnIndex = new Dictionary<string, int>(); //紀錄對應欄位索引

            for (int i = 0; i <= workbook.NumberOfSheets - 1; i++)
            {
                if (i == tablesheetIndex) continue; // 跳過TableSheet
                var sheet = workbook.GetSheetAt(i);
                var serNo = 1; 
                var ColumnDict = new Dictionary<string, int>();

                var table = new DbTable
                {
                    DatabaseName = dbName,
                    SchemaName = string.Empty,
                    TableName = string.Empty,
                    Description = string.Empty,
                    Columns = new List<DbColumn>()
                };

                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    var item = row.GetCell(0).ToString();

                    if (item == "資料表")
                    {
                        var tableFullName = row.GetCell(2).StringCellValue;
                        var (_tableSchema, _tableName) = SplitTableFullName(tableFullName);
                        table.SchemaName = _tableSchema;
                        table.TableName = _tableName;
                        table.TableFullName = tableFullName;
                    }
                    else if (item == "資料表描述")
                    {
                        var tableDescription = row.GetCell(2).StringCellValue;
                        table.Description = tableDescription;
                    }
                    else if (item == "項目")
                    {
                        int columnCount = row.LastCellNum;
                        for (int j = row.FirstCellNum; j < columnCount; j++)
                        {
                            var ColumnName = row.GetCell(j).StringCellValue;
                            int colIndex = ColumnName switch
                            {
                                "項目" => j,
                                "欄位名稱" => j,
                                "主鍵" => j,
                                "可Null" => j,
                                "自動編號" => j,
                                "資料型態" => j,
                                "預設值" => j,
                                "描述" => j,
                                _ => -1
                            };
                            if (colIndex >=0)
                                ColumnDict[ColumnName] = colIndex;
                        }
                    }
                    else if (item != "項目" && item != "")
                    {
                        var ColumnNo = serNo++;
                        //var ColumnName = row.GetCell(1).StringCellValue;
                        //var PK = row.GetCell(2).StringCellValue == "V" ? "Yes" : "No";
                        //var Nullable = row.GetCell(3).StringCellValue == "V" ? "Yes" : "No";
                        //var AutoInt = row.GetCell(4).StringCellValue == "V" ? "Yes" : "No";
                        //var FullDataType = row.GetCell(5).StringCellValue;
                        //var Default = row.GetCell(6)?.StringCellValue;
                        //var Description = row.GetCell(7)?.StringCellValue;

                        var ColumnName = row.GetCell(ColumnDict["欄位名稱"]).StringCellValue;
                        //var ColumnName = row.GetCell(1).StringCellValue;
                        var PK = ColumnDict.ContainsKey("主鍵") && row.GetCell(ColumnDict["主鍵"]).StringCellValue == "V" ? "Yes" : "No";
                        var Nullable = ColumnDict.ContainsKey("可Null") && row.GetCell(ColumnDict["可Null"]).StringCellValue == "V" ? "Yes" : "No";
                        var PKAutoInt = ColumnDict.ContainsKey("自動編號") && row.GetCell(ColumnDict["自動編號"]).StringCellValue == "V" ? "Yes" : "No";
                        var FullDataType = ColumnDict.ContainsKey("資料型態") && row.GetCell(ColumnDict["資料型態"]).StringCellValue == "" ? null : row.GetCell(ColumnDict["資料型態"]).StringCellValue;
                        var Default = ColumnDict.ContainsKey("預設值") && row.GetCell(ColumnDict["預設值"])?.StringCellValue == "" ? null : row.GetCell(ColumnDict["預設值"])?.StringCellValue;
                        var Description = ColumnDict.ContainsKey("描述") && row.GetCell(ColumnDict["描述"])?.StringCellValue == "" ? null : row.GetCell(ColumnDict["描述"])?.StringCellValue;

                        var column = new DbColumn
                        {
                            ColumnNo = serNo++,
                            //ColumnName = row.GetCell(1).StringCellValue,
                            //PK = row.GetCell(2).StringCellValue == "V" ? "Yes" : "No",
                            //Nullable = row.GetCell(3).StringCellValue == "V" ? "Yes" : "No",
                            //FullDataType = row.GetCell(4).StringCellValue,
                            //Default = row.GetCell(5)?.StringCellValue,
                            //Description = row.GetCell(6)?.StringCellValue
                            ColumnName = ColumnName,
                            PK = PK,
                            Nullable = Nullable,
                            PKAutoInt = PKAutoInt,
                            FullDataType = FullDataType,
                            Default = Default,
                            Description = Description
                        };
                        table.Columns.Add(column);
                    }
                    //tables.Add(table);
                }

                if (!tables.Contains(table))
                {
                    tables.Add(table);
                    await Task.Delay(75);
                    current++;
                    yield return (table, current, total);
                    await Task.Yield();
                }
                //var item = row.GetCell(1).StringCellValue;


                //var tableName = sheet.SheetName;

                //yield return (null, 0, 0);
                //await Task.Yield();
            }

            //var sheet = workbook.GetSheetAt(0);
            //int total = sheet.LastRowNum; // 總行數 (不包含標題列)

            // 


            //for (int i = 0; i <= workbook.NumberOfSheets - 1; i++)
            //{
            //    var sheet = workbook.GetSheetAt(i);
            //    var tableName = sheet.SheetName;



            //    var table = new DbTable
            //    {
            //        TableName = tableName,
            //        Columns = new List<DbColumn>()
            //    };
            //    for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            //    {
            //        var row = sheet.GetRow(rowIndex);
            //        if (row == null) continue;
            //        var column = new DbColumn
            //        {
            //            ColumnNo = (int)row.GetCell(0).NumericCellValue,
            //            ColumnName = row.GetCell(1).StringCellValue,
            //            IsPrimaryKey = row.GetCell(2).StringCellValue == "V",
            //            IsForeignKey = row.GetCell(3).StringCellValue == "V",
            //            FkReferencedInfo = row.GetCell(4)?.StringCellValue,
            //            IsNullable = row.GetCell(5).StringCellValue == "V",
            //            IsIdentity = row.GetCell(6).StringCellValue == "V",
            //            FullDataType = row.GetCell(7).StringCellValue,
            //            Default = row.GetCell(8)?.StringCellValue,
            //            Description = row.GetCell(9)?.StringCellValue
            //        };
            //        table.Columns.Add(column);
            //    }
            //    await Task.Delay(75);
            //    current++;
            //    yield return (table, current, total);



            //}
        }

        private (string schema, string name) SplitTableFullName(string tableFullName)
        {
            var parts = tableFullName.Split('.');
            if (parts.Length == 2)
            {
                return (parts[0], parts[1]);
            }
            else
            {
                return ("dbo", tableFullName);
            }
        }

        private bool IsTableSheet(ISheet sheet)
        {
            // 判斷是否為資料表結構的工作表，這裡假設資料表結構的工作表名稱不以 "#" 開頭
            return sheet.SheetName.StartsWith("Table");
        }
    }
}
