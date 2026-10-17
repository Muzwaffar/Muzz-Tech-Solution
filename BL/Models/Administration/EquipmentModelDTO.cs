using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class EquipmentModelListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public NPF_SYS_EquipmentMake EquipmentMake { get; set; }
        
    }

    public class EquipmentModelCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public string Name { get; set; }
        public string Description { get; set; }
        public long EquipmentMake { get; set; }
    }
}
