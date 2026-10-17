using DAL.Models.Administration;
using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Form
{
    public class ElementFormConfigDTO
    {
        public long Sequence { get; set; }
        public List<SectionConfigDTO> Sections { get; set; }
        public long? FormId { get; set; }
        public string ModuleId { get; set; } = string.Empty;

    }

    public class SectionConfigDTO
    {
        public long SectionId { get; set; }
        public string SectionName { get; set; }
        public bool ReadOnly { get; set; } = false;
        public bool Hidden { get; set; } = false;
        public List<ElementConfigDTO> Elements { get; set; }

    }
    public class ElementConfigDTO
    {
        public long ElementId { get; set; }
        public string ElementName { get; set; }
        public bool ReadOnly { get; set; } = false;
        public bool Hidden { get; set; } = false;

    }
    public class RetrieveElementFormConfigDTO
    {
        public long FormId { get; set; }
        public string ModuleId { get; set; }

    }
}
