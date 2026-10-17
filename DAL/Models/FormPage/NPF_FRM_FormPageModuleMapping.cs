using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.FormPage
{
    public class NPF_FRM_FormPageSectionModuleMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FormId { get; set; }
        public long SectionId { get; set; }
        public Guid ModuleId { get; set; }
        public bool ReadOnly { get; set; }
        public bool Hidden { get; set; }
        public long? Sequence { get; set; } = 0;

    }
}
