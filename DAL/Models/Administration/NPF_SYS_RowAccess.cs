using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_RowAccess
    {
        public long Id { get; set; }
        public long ModuleId { get; set; }
        public ModuleName ModuleName { get; set; }
        public RowStructType Struc { get; set; }
        public string StrucId { get; set; }
    }
}
