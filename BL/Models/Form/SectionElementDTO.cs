using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Form
{
    public class SectionElementDTO
    {
        public List<ElementDTO> ElemList { get; set; } = new List<ElementDTO>();
        public NPF_FRM_Section Sec { get; set; } = new NPF_FRM_Section();
    }


    public class ParamDTO
    {
        public long Id { get; set; } //Dropdown Id
        public string Name { get; set; } //Parameter 1/ Parameter 2
        public string Value { get; set; } //Query value

        public long ParamId { get; set; } // Parameter Id
    }

    public class AdditionalElementParamDTO
    {
        public NPF_FRM_Elements Element { get; set; } = new NPF_FRM_Elements();
        public List<ParamDTO> Param { get; set; } = new List<ParamDTO>();

    }
}
