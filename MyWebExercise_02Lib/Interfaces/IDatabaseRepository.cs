using System;
using System.Collections.Generic;
using System.Text;
using JackToolLib.Models;

namespace JackToolLib.Interfaces
{
    public interface IDatabaseRepository
    {
        Task<IEnumerable<string>> GetDatabaseNamesAsync();
        Task<List<DbTable>> GetTableSchemaAsync(string dbName);
    }
}
