using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ZoneDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "AUTO-GENERATED";
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public string GeomDataText { get; set; } = String.Empty;
        public int? Color { get; set; } = null;
        public long? ServiceRequestId { get; set; } = null;
        public string Type { get; set; } = String.Empty;
        public float? CenterPointLat { get; set; }
        public float? CenterPointLong { get; set; }
        public long ZoneType { get; set; }
    }

    public class ZoneListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public string ZoneType { get; set; }
    }
}
