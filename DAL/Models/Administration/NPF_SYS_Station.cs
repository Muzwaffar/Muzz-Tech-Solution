using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Station : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }

        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Name { get; set; }


        public string? Description { get; set; }

        [ForeignKey("StationTypeId")]
        public virtual NPF_SYS_StationType StationType { get; set; }

        [ForeignKey("StationLocationId")]
        public virtual NPF_SYS_Zone StationLocation { get; set; }

        [StringLength(120, ErrorMessage = "Call Sign cannot exceed 120 characters. ")]
        public string StationCallSign { get; set; }

        [StringLength(20, ErrorMessage = "Phone Number cannot exceed 20 characters. ")]
        public string PhoneNumber { get; set; }

        [StringLength(20, ErrorMessage = "Contact Number cannot exceed 20 characters. ")]
        public string ContactNumber { get; set; }

        [StringLength(20, ErrorMessage = "Fax cannot exceed 20 characters. ")]
        public string Fax { get; set; }

        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters. ")]
        public string Email { get; set; }

    }
}
