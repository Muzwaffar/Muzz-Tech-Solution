using DAL.Common;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class StationListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public NPF_SYS_StationType StationType { get; set; }
        public NPF_SYS_Zone StationLocation { get; set; }
        public string StationCallSign { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

    }

    public class StationCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public string Name { get; set; }
        public string Description { get; set; }
        public long StationType { get; set; }
        public long StationLocation { get; set; }
        public string StationCallSign { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }
}
