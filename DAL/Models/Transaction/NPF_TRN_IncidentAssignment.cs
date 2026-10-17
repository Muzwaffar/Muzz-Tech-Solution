using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Transaction
{
    public class NPF_TRN_IncidentAssignment : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long ResourceId { get; set; }
        public bool IsMain { get; set; }

    }
}
