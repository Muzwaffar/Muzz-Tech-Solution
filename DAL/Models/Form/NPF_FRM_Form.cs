using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_Form : BaseAuditModel, IAuditable
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        [ForeignKey("FormTypeId")]
        public virtual NPF_FRM_FormType FormType { get; set; } 
        public long? ParentId { get; set; }
        public bool? IsMobile { get; set; } = false;
        public Guid? ModuleId { get; set; } 
    }
}
