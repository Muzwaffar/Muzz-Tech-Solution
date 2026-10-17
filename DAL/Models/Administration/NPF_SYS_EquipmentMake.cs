using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_EquipmentMake : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }

        [StringLength(120, ErrorMessage = "Asset Make cannot exceed 120 characters. ")]
        public string Name { get; set; } = String.Empty;

        public string? Description { get; set; }
    }
}
