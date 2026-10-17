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
    public class NPF_TRN_IncidentLocation : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Address { get; set; }
        public string Street { get; set; } 
        public string Locality { get; set; }
        public string District { get; set; }
        public string LandMark { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public long? Buffer { get; set; }
        public string GeomDataText { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;

    }
}
