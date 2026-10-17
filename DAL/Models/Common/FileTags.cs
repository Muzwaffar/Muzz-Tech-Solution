using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Common
{
    public class FileTags : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FileIndexId { get; set; }
        public string Tag { get; set; }
    }
}
