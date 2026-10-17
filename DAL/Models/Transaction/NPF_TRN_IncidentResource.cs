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
    public class NPF_TRN_IncidentResource : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long ResourceId { get; set; }
        public ResourceStatus ResourceStatus { get; set; } = ResourceStatus.NoShow;
        public bool IsCrewCommander { get; set; } = false;
        public long? StationId { get; set; } = 0;
        public string? StationName { get; set; } = string.Empty;


    }
}
