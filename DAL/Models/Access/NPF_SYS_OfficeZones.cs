using DAL.Common;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Access
{
    public class NPF_SYS_OfficeZones : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string OfficeName { get; set; }
        public long Department { get; set; }
        public long Zone { get; set; }
        public long Locality { get; set; }
        public string Abbrevation { get; set; } = string.Empty;
    }
}
