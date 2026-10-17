using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_SourceType : BaseAuditModel, IAuditable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
