using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ZoneTypeDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "AUTO-GENERATED";
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
