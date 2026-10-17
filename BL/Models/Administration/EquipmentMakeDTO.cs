using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class EquipmentMakeListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class EquipmentMakeCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
