using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Builder
{
    public class Element
    {
        public string ElementTypeName { get; set; }
        public string HtmlType { get; set; }
        public string ElementId { get; set; }
        public string ElementName { get; set; }
        public List<Element> SubElems { get; set; } = new List<Element>();
        public List<ElementOption> List { get; set; } = new List<ElementOption>();
        public List<string> ListValue { get; set; } = new List<string>();
    }

    public class ElementOption
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }

    public class SubSection
    {
        public string SectionCode { get; set; }
        public string SubSectionCode { get; set; }
        public string SubSectionName { get; set; }
        public List<Element> Elements { get; set; } = new List<Element>();
    }

    public class RootObject
    {
        public List<SubSection> Sections { get; set; } = new List<SubSection>();
    }
}
