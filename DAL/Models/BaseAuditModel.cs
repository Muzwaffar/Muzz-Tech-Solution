using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BaseAuditModel
    {

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Boolean DeleteStatus { get; set; }
    }
}
