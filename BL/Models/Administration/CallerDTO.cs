using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class CallerListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NIC { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public bool IsNuisance { get; set; } = false;
        public string? Remarks { get; set; } = string.Empty;
    }

    public class CallerCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long NAAId { get; set; }
        public string NAACode { get; set; }
        public string NICPassport { get; set; }

        public string? MobileNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? Street { get; set; }
        public string? Locality { get; set; } = String.Empty;
        public long? LocalityId { get; set; }
        public string District { get; set; } = String.Empty;
        public long? DistrictId { get; set; }
        public bool IsNuisance { get; set; } = false;
        public string? Remarks { get; set; } = string.Empty;
    }
}
