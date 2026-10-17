using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Common
{
    public class EmailConfiguration : BaseAuditModel, IAuditable
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Host { get; set; } = string.Empty;
    }
}
