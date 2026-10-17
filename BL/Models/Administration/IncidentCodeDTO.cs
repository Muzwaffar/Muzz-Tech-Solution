using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class IncidentCodeDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "AUTO-GENERATED";
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public long IncidentSubType { get; set; }
        public string IncidentType { get; set; }
    }

    public class IncidentCodeListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public string IncidentSubType { get; set; } 
        public string IncidentType { get; set; } = String.Empty;
    }
}
