using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_CompanyList : BaseAuditModel, IAuditable
    {
        [Key]
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string AdditionalValue { get; set; } = String.Empty;
        public string Comment { get; set; }
        public bool Enable { get; set; } = false;
    }
}