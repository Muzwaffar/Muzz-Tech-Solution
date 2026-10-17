using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Form
{
    public class FormDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public long FormTypeId { get; set; }
    }

    public class FormSectionDTO
    {
        public NPF_FRM_Form Form { get; set; } = new NPF_FRM_Form();
        public List<NPF_FRM_Section> Sec { get; set; } = new List<NPF_FRM_Section>();
    }

    public class FormListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public string FormTypeName { get; set; }
    }
}
