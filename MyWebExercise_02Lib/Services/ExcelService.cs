using JackToolLib.Models;
using NPOI.XSSF.UserModel;    
using System;
using System.Collections.Generic;
using System.Text;
using JackToolLib.Extension;

namespace JackToolLib.Services
{
    public class ExcelService
    {
        public void GenerateDocument(List<DbTable> DbTables, List<string> tableNames, string templatePath, string outputDocPath)
        {
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
                    tableSheet.SetFirstMatchCellHyperlinkInRow(newRowIndex, tagName4TableName, table.TableName, tableIndex.ToString());
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableDesc, table.Description);
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableView, table.IsViewTable ? "V" : "");
                    tableSheet.SetFirstMatchCellContentInRow(newRowIndex, tagName4TableType, table.TableType.ToTitleCase());

                    tableIndex++;
                }

            }

            // remove the last template row
            tableSheet.RemoveFirstMatchRow(tagName4TableNo);


            // TABLE SCHEMA SHEETS

            tableIndex = 1;
            foreach (var table in DbTables.Where(t => tableNames.Contains(t.TableName)))
            {
                // clone tempalte schema sheet & rename sheet
                var schemaSheet = workbook.CloneSheet(workbook.GetSheetIndex(sheetName4TableSchemaTempalte));
                workbook.SetSheetName(workbook.NumberOfSheets - 1, tableIndex.ToString()/*table.TableName*/);

                // table name & description
                schemaSheet.SetFirstMatchCellContent(tagName4TableName, table.TableName);
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

                tableIndex++;
            }

            // remove template sheet
            workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName4TableListTempalte));
            workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName4TableSchemaTempalte));

            // save as another excel file
            using (FileStream stream = new FileStream(outputDocPath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(stream);
            }

            // set null 
            workbook = null;
        }
    }
}
