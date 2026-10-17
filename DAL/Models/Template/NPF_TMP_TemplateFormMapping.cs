using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Template
{
    public class NPF_TMP_TemplateFormMapping : BaseAuditModel, IAuditable
    {

        [Key]
        public long Id { get; set; }
        public string TemplateName { get; set; }
        public Guid? ModuleId { get; set; }
        public string? URL { get; set; }
        public long FormId { get; set; }
    }
}
