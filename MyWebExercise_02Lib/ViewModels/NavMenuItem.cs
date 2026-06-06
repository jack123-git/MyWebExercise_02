using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.ViewModels
{
    public class NavMenuItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public List<NavMenuItem> SubMenu { get; set; } = new();
    }
}
