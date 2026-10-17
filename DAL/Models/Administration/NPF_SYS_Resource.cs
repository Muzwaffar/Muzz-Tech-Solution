using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Resource : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }

        [ForeignKey("NAAId")]
        public virtual NPF_DIR_NAA NAA { get; set; }

        [ForeignKey("ResourceTypeId")]
        public virtual NPF_SYS_ResourceType ResourceType { get; set; }
        public long? UserId { get; set; }


        [ForeignKey("RankId")]
        public virtual NPF_SYS_Rank Rank { get; set; }

        public DateTime? DatejoinedMFRS { get; set; }

        [StringLength(120, ErrorMessage = "PF Number cannot exceed 120 characters. ")]
        public string PFNumber { get; set; } = String.Empty;

        [ForeignKey("StationUnitId")]
        public virtual NPF_SYS_Station StationUnitName { get; set; }

        [StringLength(120, ErrorMessage = "Private License Number cannot exceed 120 characters. ")]
        public string? PrivateLicenseNumber { get; set; } = String.Empty;

        public long? PrivateLicenseDriverCategoryId { get; set; }

        public DateTime? PrivateLicenseIssueDate { get; set; }

        public DateTime? PrivateLicenseExpiryDate { get; set; }

    }
}
