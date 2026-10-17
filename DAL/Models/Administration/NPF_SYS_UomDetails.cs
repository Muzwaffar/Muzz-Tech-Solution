using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_UomDetails: BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Name { get; set; }

        [ForeignKey("UomHeaderId")]
        public virtual NPF_SYS_UomHeader UomHeader { get; set; }
        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Unit { get; set; }
        public bool IsSystemInternational { get; set; }
        public float RelativeFactor { get; set; }
    }
}
