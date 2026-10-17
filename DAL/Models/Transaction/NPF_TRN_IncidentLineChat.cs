using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Transaction
{
    public class NPF_TRN_IncidentLineChat : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Line { get; set; }
        public string Type { get; set; } = string.Empty;

    }
}
