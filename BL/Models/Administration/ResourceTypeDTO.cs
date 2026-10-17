using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ResourceTypeListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; } = false;
        public bool IgnoreStation { get; set; } = false;
    }
    public class ResourceTypeCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IgnoreStation { get; set; }
    }
}
