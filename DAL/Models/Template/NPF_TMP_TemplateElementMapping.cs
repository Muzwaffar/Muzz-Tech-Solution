using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Template
{
    public class NPF_TMP_TemplateElementMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FkTFM { get; set; }
        public long ElementId { get; set; }
        public long? ParentId { get; set; }
        public string ElementType { get; set; }
        public string TextName { get; set; }
        public long Column { get; set; } = 0;
        public long Row { get; set; } = 0;
    }
}
