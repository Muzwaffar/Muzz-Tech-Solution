using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class NatureOfIncidentListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class NatureOfIncidentCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
