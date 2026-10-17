using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_PDA : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }

        [ForeignKey("StationTypeId")]
        public virtual NPF_SYS_StationType StationType { get; set; }

        [ForeignKey("NatureOfIncidentId")]
        public virtual NPF_SYS_NatureOfIncident NatureOfIncident { get; set; }

        [ForeignKey("IncidentTypeId")]
        public virtual NPF_SYS_IncidentType? IncidentType { get; set; }
    }
}
