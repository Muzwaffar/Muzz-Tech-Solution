using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Administration;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Common;

namespace DAL.Models.Transaction
{
    public class NPF_TRN_IncidentCasualtyDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public string Remarks { get; set; } = string.Empty;
        public long? Gender { get; set; }
        public long? CasualtyType { get; set; }

    }
}
