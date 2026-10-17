using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_LookUpType : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Name { get; set; }
        [StringLength(120, ErrorMessage = "Description cannot exceed 120 characters. ")]
        public string Description { get; set; }
    }
}
