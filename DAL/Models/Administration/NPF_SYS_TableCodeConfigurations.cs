using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_TableCodeConfigurations : BaseAuditModel, IAuditable
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        public string Prefix { get; set; }
        public int CompanyId { get; set; }
        public int ConfigurationId { get; set; }
        public bool? HasAddi { get; set; } = false;
        public string Comment { get; set; } = String.Empty;

    }
}
