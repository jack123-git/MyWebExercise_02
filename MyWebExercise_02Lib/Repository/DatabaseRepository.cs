using JackToolLib.Data;
using JackToolLib.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using JackToolLib.Models;

namespace JackToolLib.Repository
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly SqlDataAccess sqlDataAccess;

        public DatabaseRepository(IConfiguration config)
        {
            this.sqlDataAccess = new SqlDataAccess(config);
        }

        public async Task<IEnumerable<string>> GetDatabaseNamesAsync()
        {
            var result =await sqlDataAccess.GetDatabaseNamesAsync();
            return result;
        }

        public async Task<List<DbTable>> GetTableSchemaAsync(string dbName)
        {
            var result = await sqlDataAccess.GetTableSchemaAsync(dbName);
            return result;
        }
    }
}
