using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_GroupMatrix : BaseAuditModel, IAuditable
    {
        [Key]
        public long GMID { get; set; }
        public long? ParentGMID { get; set; }
        public string GMDescription { get; set; }
        public string Remarks { get; set; }
        public bool IsCompany { get; set; }
        public string? CompanyCode { get; set; }
        public string? LegalName { get; set; }
    }
}
