using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Research
{
    public class NPF_RES_ApprovalUsers : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RowId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public ResearchSubModule ResearchSubModule { get; set; }
    }
}
