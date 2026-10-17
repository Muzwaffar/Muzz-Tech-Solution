using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Roster : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters. ")]
        public string Name { get; set; } = String.Empty;
        [StringLength(75, ErrorMessage = "Description cannot exceed 75 characters. ")]
        public string Description { get; set; } = String.Empty;
        public DateTime RosterFrom { get; set; } = DateTime.Now;
        public DateTime RosterTo { get; set; } = DateTime.Now;

        [NotMapped]
        public string RosterPeriod { get { return RosterFrom.ToString().Substring(0, RosterFrom.ToString().IndexOf(" ")) + " - " + RosterTo.ToString().Substring(0, RosterTo.ToString().IndexOf(" ")); } }
    }
}
