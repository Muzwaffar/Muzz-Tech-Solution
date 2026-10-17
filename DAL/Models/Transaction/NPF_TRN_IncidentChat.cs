using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Transaction
{
    public class NPF_TRN_IncidentChat : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public long IncidentId { get; set; }
        public string Text { get; set; }
        public string TimeStamp { get; set; }
    }
}
