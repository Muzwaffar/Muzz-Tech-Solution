using DAL.Common;
using DAL.Models.Access;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_AccessRightsDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("AccessRightId")]
        public virtual NPF_SYS_AccessRights AccessRights { get; set; }
        public AccessOperationType AOT { get; set; }
        public bool Permission { get; set; } = false;   

    }
}
