using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_InputTypeDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDatatable { get; set; }
        [ForeignKey("InputTypeId")]
        public virtual NPF_FRM_InputType InputType { get; set; }
    }
}
