using DAL.Common;
using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Store
{
    public class StoreItemListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Group { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string IsConsumable { get; set; }
        public string UomDetails { get; set; }
    }
    public class StoreItemDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public long Group { get; set; }
        public long Type { get; set; }
        public long? Category { get; set; }
        public long? SubCategory { get; set; }
        public bool IsConsumable { get; set; }
        public long UOM { get; set; }
        public long UomDetails { get; set; }
        public long AnimalProductType { get; set; }
        public long? ParentItem { get; set; }

    }
}
