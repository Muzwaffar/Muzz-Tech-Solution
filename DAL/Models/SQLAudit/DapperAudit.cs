using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.SQLAudit
{

    [Table("DapperAudit")]
    public class DapperAudit
    {
        [Key]
        public long Id { get; set; }
        public string userId { get; set; }
        public string QueryExcecuted { get; set; }
        public DateTime ExecutedDate { get; set; }
        public string LastKnownIPAddress { get; set; }
    }
}
