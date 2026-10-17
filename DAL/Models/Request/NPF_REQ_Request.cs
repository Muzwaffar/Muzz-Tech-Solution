using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Request
{
    public class NPF_REQ_Request : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long EntityId { get; set; }
        public MRequestType Type { get; set; }
        public int Quantity { get; set; } = 0;
        public string Additional { get; set; } = string.Empty;
        public bool Completed { get; set; } = false;
    }
}
