using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_IncidentCode :BaseAuditModel ,IAuditable
    {
        [Key]
        public long Id { get; set; }
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;

        [ForeignKey("IncidentSubTypeId")]
        public virtual NPF_SYS_IncidentSubType IncidentSubType { get; set; }

        [StringLength(50, ErrorMessage = "IncidentType cannot exceed 50 characters. ")]
        public string IncidentType { get; set; }
    }
}
