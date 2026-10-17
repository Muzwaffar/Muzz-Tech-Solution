using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Access
{
    public class NPF_SYS_AccessRights : BaseAuditModel,IAuditable
    {
        [Key]
        public Guid id { get; set; }
        public bool hasRight { get; set; } = false;

        [ForeignKey("RoleId")]
        public virtual ApplicationRole Role { get; set; }

        [ForeignKey("ModuleId")]
        public virtual NPF_SYS_Modules Module{ get; set; }


      
    }
}
