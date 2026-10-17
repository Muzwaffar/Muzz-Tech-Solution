using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class PriorityListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

    }
    public class PriorityDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } = "Auto-Generated";
        public string Description { get; set; } 
    }
}
