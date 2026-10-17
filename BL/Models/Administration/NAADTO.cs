using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class NAAlistDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
    }
    public class NAACRUDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public int Title { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Identifier { get; set; }
        public string NICPASS { get; set; } = String.Empty;
        public int? Gender { get; set; }
        public DateTime? DOB { get; set; } = DateTime.Now;
        public string? Email { get; set; }
        public string? TelNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string? Street { get; set; }
        public string? Locality { get; set; }
        public long? LocalityId { get; set; }
        public string? VillageTown { get; set; }
        public long? VillageTownId { get; set; }
        public string? District { get; set; }
        public long? DistrictId { get; set; }
    }
}
