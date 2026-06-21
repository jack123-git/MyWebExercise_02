using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Data
{
    public class CommonDataAccess(IConfiguration config)
    {
        private readonly IConfiguration _config = config;
        private readonly string _systemDbName = "CommonDbConnection";


    }
}
