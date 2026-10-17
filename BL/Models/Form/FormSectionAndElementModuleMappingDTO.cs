using BL.Models.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Form
{
    public class FormSectionAndElementModuleMappingDTO
    {
        public long Id { get; set; }
        public long Form { get; set; }
        public string Module { get; set; }
        public List<ElementMappingDTO> Element { get; set; }
    }
    public class ElementMappingDTO
    {
        public long Id { get; set; }
        public long ElementId { get; set; }
        public string ElementName { get; set; } = string.Empty;
        public bool ReadOnly { get; set; }
    }
    public class FormSectionAndElementModuleMappingListDTO
    {
        public long Id {  get; set; }
        public string Form {  get; set; }
        public string Module {  get; set; }
    }
    public class FormSectionAndElementModuleMappingCRUDTO
    {
        public long Form { get; set; }
        public Guid Module { get; set; }
    }
    public class FormSectionAndElementModuleMappingTableDTO
    {
        public long Form { get; set; }
        public string Module { get; set; }
    }
}
