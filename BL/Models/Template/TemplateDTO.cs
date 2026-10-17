using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Template
{
    public class TemplateDTO
    {
        public string sInput { get; set; }
        public string sMultiline { get; set; }
        public int sNumber { get; set; }
        public DateTime sTime { get; set; }
        public DateTime sDate { get; set; }
        public DateTime sDatetime { get; set; }
        public string sDropdown { get; set; }
        public string sMultiselect { get; set; }
        public bool sCheckbox { get; set; }
        public string sRadio { get; set; }

    }

    public class TemplateFormMappingDTO
    {
        public long Id { get; set; }
        public string TemplateName { get; set; }
        public string ModuleId { get; set; } = string.Empty;
        public long FormId { get; set; }

        public List<TemplateElementMappingDTO> Element { get; set; }
    }
    public class TemplateFormMappingListDTO
    {
        public long Id { get; set; }
        public string TemplateName { get; set; }
        public string FormName { get; set; }
    }

    public class TemplateFormMappingCRUDTO
    {
        public long Id { get; set; }
        public string TemplateName { get; set; }
        public long FormId { get; set; }
        public Guid ModuleId { get; set; }
    }

    public class TemplateElementMappingDTO
    {
        public long Id { get; set; }

        public long FkTFM { get; set; }
        public long ElementId { get; set; }
        public long? ParentId { get; set; }
        public long? Row { get; set; }
        public long? Column { get; set; }
        public string TextName { get; set; }
        public string ElementName { get; set; }
        public long ElementTypeId { get; set; }
        public string ElementType { get; set; }

    }

    public class TemplateDownloadDTO
    {
        public long Id { get; set; }
        public string? url { get; set; } = string.Empty;
    }

}
