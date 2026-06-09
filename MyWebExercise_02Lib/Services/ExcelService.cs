using JackToolLib.Extension;
using JackToolLib.Interfaces;
using JackToolLib.Models;
using NPOI.XSSF.UserModel;

namespace JackToolLib.Services
{
    public class ExcelService: IExcelService
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
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnNo, column.No.ToString());
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnName, column.ColumnName);
                        schemaSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4ColumnPK, column.IsPrimaryKey ? "V" : "");
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
    }
}
