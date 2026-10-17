using DAL.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Zone : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters. ")]
        public string Name { get; set; }
        [StringLength(75, ErrorMessage = "Description cannot exceed 75 characters. ")]
        public string Description { get; set; } = String.Empty;
        public string GeomDataText { get; set; } = String.Empty;
        public int? Color { get; set; } = null;
        public long? ServiceRequestId { get; set; } = null;
        public string Type { get; set; } = String.Empty;
        public float? CenterPointLat { get; set; } 
        public float? CenterPointLong { get; set; }

        [ForeignKey("ZoneTypeId")]
        public virtual NPF_SYS_ZoneType ZoneType { get; set; }
    }

    


}
