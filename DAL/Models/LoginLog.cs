using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("LoginLog")]
    public class LoginLog
    {
        [Key]
        public long Id { get; set; }
        public string userId { get; set; }
        public string Description { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string LastKnownIPAddress { get; set; }
    }
}
