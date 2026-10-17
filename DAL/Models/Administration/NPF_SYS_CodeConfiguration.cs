using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_CodeConfiguration : BaseAuditModel, IAuditable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Date { get; set; } = false;
        public string DateFormat { get; set; } = string.Empty;
        public bool Month { get; set; } = false;
        public string MonthFormat { get; set; } = string.Empty;
        public bool Year { get; set; } = false;
        public string YearFormat { get; set; } = string.Empty;
        public bool UsePrefix { get; set; } = false;
        public bool? Reset { get; set; } = false;
        public string ResetConfig { get; set; } = string.Empty;
        public int PaddingNo { get; set; }
        public string Comment { get; set; } = String.Empty;

    }
}

