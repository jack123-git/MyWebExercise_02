using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Models
{
    public class MyDbModel
    {
        public string NameSpace { get; set; }
        public List<DbTable> Tables { get; set; }
        public List<string> Usings { get; set; } = new List<string>();
    }
}
