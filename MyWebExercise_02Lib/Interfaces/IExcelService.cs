using JackToolLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> GenerateDocumentAsync(List<DbTable> DbTables, List<string> tableNames, string templatePath, IProgress<(int current, int total)> progress);

        IAsyncEnumerable<(DbTable table, int current, int total)> ImportExcelTableSchemaAsync(Stream fileStream, string fileName);
    }
}
