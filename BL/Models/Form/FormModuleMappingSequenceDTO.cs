using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Form
{
    public class FormModuleMappingSequenceDTO
    {
        public long Id { get; set; }
        public Guid Module { get; set; }
        public long Form { get; set; }
        public long Sequence { get; set; }
    }

    public class FormModuleMappingSequenceListDTO
    {
        public long Id { get; set; }
        public string Module { get; set; }
        public string FormName { get; set; }
        public long Sequence { get; set; }
    }
}
