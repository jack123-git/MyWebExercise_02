using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackToolLib.Models
{
    public class DbTable
    {
        // Properties
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string TableFullName { get; set; } // SchemaName.TableName

        public string Description { get; set; }

        public string TableType { get; set; }

        public bool IsViewTable
        {
            get
            {
                return TableType != null
                  && TableType.IndexOf("VIEW", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public List<DbColumn> Columns { get; set; }


        // Constructor
        public DbTable()
        {
            Columns = new List<DbColumn>();
        }


        // Methods
        public override string ToString()
        {
            return TableName + (IsViewTable ? " (view)" : "");
        }
    }
}
