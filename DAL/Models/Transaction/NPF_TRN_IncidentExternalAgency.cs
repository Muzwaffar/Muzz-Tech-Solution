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
    public class NPF_TRN_IncidentExternalAgency : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long ExternalAgencyId { get; set; }
        public string Remarks { get; set; }

    }
}
