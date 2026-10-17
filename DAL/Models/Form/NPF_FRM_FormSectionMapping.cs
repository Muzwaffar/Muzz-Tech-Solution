using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_FormSectionMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long SectionId { get; set; }
        public long FormId { get; set; }
        public int? Order { get; set; }
    }
}
