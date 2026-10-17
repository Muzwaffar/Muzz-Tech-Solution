
using BL.Models.Common;
using DAL.Common;
using DAL.Models.FormPage;
using iTextSharp.text;

namespace BL.Models.FormPage
{
    public class FormPageDTO
    {
        public long FormPageId { get; set; }
        public long FormId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public string Print { get; set; } = string.Empty;
        public Status Status { get; set; }
        public ApprovalStatus ApprovalS { get; set; } = ApprovalStatus.Draft;
        public decimal? TotalAmount { get; set; }
        public List<DTSection> Sections { get; set; } = new List<DTSection>();
        public DropDown? DropDown { get; set; } = new DropDown();
        public ApprovalStatus Approval { get; set; } = ApprovalStatus.Draft;
        public GeneralStatus GeneralStatus { get; set; } = GeneralStatus.Open;
        public bool IsApprover { get; set; } = false;

    }

    public class FormPageListDTO
    {
        public List<NPF_FRM_FormPage> FormPageDetails { get; set; } = new List<NPF_FRM_FormPage> { };
    }

    public class SectionElements
    {
        public ElementList ElementList { get; set; } = new ElementList();
        public List<DTSection> Sections { get; set; } = new List<DTSection>();
    }
    public class ElementList
    {
        public List<Elements> Elements { get; set; } = new List<Elements>();
    }

    public class DTSection
    { 
        public long sectionId { get; set; }
        public string name { get; set; }
        public bool print { get; set; }
        public decimal amount { get; set; } = 0;
        public bool notification { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public bool Hidden { get; set; } = false;
        public List<Elements> elements { get; set; } = new List<Elements>();
    }
    public class Elements
    {
        public long? id { get; set; }
        public long elementId { get; set; }
        public string elementName { get; set; } = string.Empty;
        public string elementType { get; set; } = string.Empty;
        public string uom { get; set; } = string.Empty;
        public List<Values> uomlist { get; set; } = new List<Values>();
        public string value { get; set; } = string.Empty;
        public string additionalValue { get; set; } = string.Empty;
        public List<Values> sourceValues { get; set; } = new List<Values>();
        public long? additionalDetailId { get; set; } = null;
        public int? row { get; set; } = null;
        public int? order { get; set; } = null;
        public int? column { get; set; } = null;
        public long? entityid { get; set; } = null; // selected dropdown Id
        public long? entitysourceId { get; set; } = null; //dropdown table id (already in dto)
        public string? entityname { get; set; } = null; //table name (already in dto)
        public bool? isvalue { get; set; } = null;
        public long? parentid { get; set; } = null;
        public List<Elements> innerelements { get; set; } = new List<Elements>();
        public List<Elements> innerelementsvalue { get; set; } = new List<Elements>();
        public string? url { get; set; } = null;
        public bool ReadOnly { get; set; } = false;
        public bool Hidden { get; set; } = false;
    }
    public class Values
    {
        public long id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; } = false;
        public bool disabled { get; set; } = false ;
    }
    public class FormPageRangeForm
    {
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;
        public string Plan { get; set; } = string.Empty;
    }

    public class CustomDictionary
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class DetailsByIdSearchDTO
    {
        public long ElementId { get; set; }
        public long DetailsId { get; set; }
    }
}
