using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public class DropDown
    {
        public string title { get; set; }

        public List<DropDownItem> items { get; set; }
    }
    public class DropDownApi
    {
        public long id { get; set; }
        public string name { get; set; }

    }
    public class DropDownItem
    {

        public long Id { get; set; }

        public string text { get; set; }
        public string  Additional { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public string lot { get; set; } = String.Empty;
        public int? Qty { get; set; } 
        public bool? IsAvailable { get; set; } 
        public bool IsSelected { get; set; } = false;
    }
    public class DropDownLogin
    {
        public string title { get; set; }

        public List<DropDownItemLogin> items { get; set; }
    }

    public class DropDownItemLogin
    {

        public string id { get; set; }

        public string text { get; set; }
        public string Additional { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public string lot { get; set; } = String.Empty;
        public int? Qty { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsSelected { get; set; } = false;
    }

    public class DropdownForm
    {
        public string term { get; set; }
        public string title { get; set; }
        public long? param { get; set; }
    }
}
