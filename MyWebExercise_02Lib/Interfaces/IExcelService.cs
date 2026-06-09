using JackToolLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> GenerateDocumentAsync(List<DbTable> DbTables, List<string> tableNames, string templatePath, IProgress<(int current, int total)> progress);
    }
}
