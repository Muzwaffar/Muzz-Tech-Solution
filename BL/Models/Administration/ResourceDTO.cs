using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ResourceListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Station { get; set; }
        public string Rank { get; set; }
        public string PFNumber { get; set; }
        public string ResourceType { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
    }
    public class ResourceUserDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }

    }
    public class ResourceCRUDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long ResourceType { get; set; }
        public long Title { get; set; }
        public long NAAId { get; set; }
        public string NAACode { get; set; }
        public string NICPassport { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; } = DateTime.Now;
        public long? UserId { get; set; }
        public long? Gender { get; set; }
        public string Email { get; set; }
        public string? TelNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Street { get; set; }
        public string? Locality { get; set; }
        public string? VillageTown { get; set; }

        public long? Rank { get; set; }
        public DateTime? DatejoinedMFRS { get; set; }
        public string PFNumber { get; set; } = String.Empty;
        public long? StationUnitName { get; set; }
        public string? PrivateLicenseNumber { get; set; } = String.Empty;
        public long? PrivateLicenseDriverCategoryId { get; set; }
        public DateTime? PrivateLicenseIssueDate { get; set; }

        public DateTime? PrivateLicenseExpiryDate { get; set; }
    }

    public class ResourceNAADTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long Title { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Identifier { get; set; }
        public long? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? Email { get; set; }
        public string? TelNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string? Street { get; set; }
        public string? Locality { get; set; }
        public string? VillageTown { get; set; }
    }
    public class ResourceRosterListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NIC { get; set; }
        public long ResourceType { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; }
    }
}
