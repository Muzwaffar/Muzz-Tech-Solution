using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_ParametersList : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? IsComplex { get; set; } = false;
        public long SourceId { get; set; }

    }
}
