using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.FormPage
{
    public class NPF_FRM_FormPageElementModuleMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long Fk_FormSectionModuleId { get; set; }
        public long ElementId { get; set; }
        public bool ReadOnly { get; set; }
        public bool Hidden { get; set; }
        public bool? IsListing { get; set; } = false;

    }
}
