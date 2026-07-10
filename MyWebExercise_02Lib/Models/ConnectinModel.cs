using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackToolLib.Models
{
    public class ConnectinModel
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConnectionString { 
            get { 
                return $"Data Source={Server};User ID={UserName};Password={Password};Initial Catalog={Database};Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"; 
            }  
        }        
    }
}
