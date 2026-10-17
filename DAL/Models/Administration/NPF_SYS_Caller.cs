using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Caller : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }

        [ForeignKey("NAAId")]
        public virtual NPF_DIR_NAA NAA { get; set; }

        public bool IsNuisance { get; set; } = false;

        public string? Remarks { get; set; } = String.Empty;
    }

}
