using DAL.Models.Administration;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class PDADTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;

        public  long StationTypeId { get; set; }
        public string StationTypeName { get; set; } = string.Empty;
        public long NatureOfIncidentId { get; set; }
        public string NatureOfIncidentName { get; set; } = string.Empty;
        public long IncidentTypeId { get; set; }
        public string IncidentTypeName { get; set; } = string.Empty;

        public List<PDAVehicleMappingDTO> PDAVehicleMappingDTO { get; set; }
        public List<PDAEquipmentMappingDTO> PDAEquipmentMappingDTO { get; set; }
        public List<PDAResourceMappingDTO> PDAResourceMappingDTO { get; set; }
    }

    public class PDAVehicleMappingDTO
    {
        public long Id { get; set; }

        public NPF_SYS_LookUpValue VehicleCategory { get; set; }

        public long VehicleQuantity { get; set; }

        public long Fk_PDA { get; set; }

    }
    public class PDAEquipmentMappingDTO
    {
        public long Id { get; set; }

        public NPF_SYS_LookUpValue EquipmentCategory { get; set; }

        public long EquipmentQuantity { get; set; }

        public long Fk_PDA { get; set; }
    }

    public class PDAResourceMappingDTO
    {
        public long Id { get; set; }

        public NPF_SYS_Rank Name { get; set; }
        public long RankQuantity { get; set; }

        public long Fk_PDA { get; set; }
    }

}
