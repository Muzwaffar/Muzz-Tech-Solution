using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DAL.Models.Activities;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Models.Administration;

namespace DAL.Models.Store
{
    public class NPF_STR_Store : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [ForeignKey("StationId")]
        public virtual NPF_SYS_Station Station { get; set; }
    }
}
