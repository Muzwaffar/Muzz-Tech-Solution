using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Division : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual NPF_SYS_Department Department { get; set; }
    }
}
