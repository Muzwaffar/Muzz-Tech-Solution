using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_DIR_NAA : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public Guid GId { get; set; } = Guid.NewGuid();
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }
        [StringLength(120, ErrorMessage = "FirstName cannot exceed 120 characters. ")]
        public string FirstName { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "MiddleName cannot exceed 120 characters. ")]
        public string MiddleName { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "LastName cannot exceed 120 characters. ")]
        public string LastName { get; set; } = String.Empty;
        public int? Title { get; set; }
        public int? Gender { get; set; }
        public string Identifier { get; set; } = String.Empty;
        public string NICPASS { get; set; } = String.Empty;
        public DateTime? DOB { get; set; } = DateTime.Now; //DOB has been changed to nullable on 2023-03-01
        public int? MaritalSatus { get; set; }
        [StringLength(20, ErrorMessage = "TelephoneNo cannot exceed 20 characters. ")]
        public string TelephoneNo { get; set; } = String.Empty;
        [StringLength(20, ErrorMessage = "TelephoneNo2 cannot exceed 20 characters. ")]
        public string TelephoneNo2 { get; set; } = String.Empty;
        [StringLength(20, ErrorMessage = "MobileNo cannot exceed 20 characters. ")]
        public string MobileNo { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Email cannot exceed 120 characters. ")]
        public string Email { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Religion cannot exceed 120 characters. ")]
        public string Religion { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Occupation cannot exceed 120 characters. ")]
        public string Occupation { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Address cannot exceed 120 characters. ")]
        public string Address { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "HouseNo cannot exceed 120 characters. ")]
        public string HouseNo { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Street cannot exceed 120 characters. ")]
        public string Street { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "OtherAddress cannot exceed 120 characters. ")]
        public string OtherAddress { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Locality cannot exceed 120 characters. ")]
        public string Locality { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "VillageTown cannot exceed 120 characters. ")]
        public long? LocalityId { get; set; }
        public string VillageTown { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Country cannot exceed 120 characters. ")]
        public string Country { get; set; } = String.Empty;
        [StringLength(50, ErrorMessage = "District cannot exceed 50 characters. ")]
        public string District { get; set; } = String.Empty;
        public long? DistrictId { get; set; }
        public int GMID { get; set; } = 0;
        [StringLength(20, ErrorMessage = "Fax cannot exceed 120 characters. ")]
        public string Fax { get; set; } = String.Empty;
        public int StatsId { get; set; } = 0;
        public int BId { get; set; } = 0;
        [NotMapped]
        public string FullName { get { return FirstName + " " + LastName; } }
        [NotMapped]
        public string FullNameM { get { return FirstName + " " + MiddleName + " " + LastName; } }
    }
}
