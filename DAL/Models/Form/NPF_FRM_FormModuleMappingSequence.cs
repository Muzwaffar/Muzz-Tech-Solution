using DAL.Models.Access;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_FormModuleMappingSequence : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("ModuleId")]
        public virtual NPF_SYS_Modules Module { get; set; }
        [ForeignKey("FormId")]
        public virtual NPF_FRM_Form Form { get; set; }
        public long Sequence { get; set; }
        public long? ParentId { get; set; } = 0;
        public bool? HasApproval { get; set; } = false;
    }
}
