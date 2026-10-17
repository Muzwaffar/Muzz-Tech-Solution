using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Administration;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Common;

namespace DAL.Models.Store
{
    public class NPF_STR_StoreItem : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsConsumable { get; set; }
        public long UomDetails { get; set; }
        public AnimalProducts AnimalProductType { get; set; }
        public long? ParentItem { get; set; } 

    }
}
