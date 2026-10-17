using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Access
{
    public class OfficeZonesListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto-generated";
        public string OfficeName { get; set; }
        public string Department { get; set; }
        public string Locality { get; set; }
        public string Zone { get; set; }
    }

    public class OfficeZonesCRUDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto-generated";
        public string OfficeName { get; set; }
        public long Department { get; set; }
        public long Locality { get; set; }
        public long Zone { get; set; }
        public string Abbreviation { get; set; }
    }
}
