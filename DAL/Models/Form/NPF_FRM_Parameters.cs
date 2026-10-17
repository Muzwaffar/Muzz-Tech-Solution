using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_Parameters : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [ForeignKey("ParameterListId")]
        public virtual NPF_FRM_ParametersList ParameterList { get; set; }
        public long ElementId { get; set; }
        public string? Value { get; set; }

    }
}