using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Store
{
    public class NPF_STR_StoreMatrix : BaseAuditModel, IAuditable
    {
        [Key]
        public long MID { get; set; }
        public long IID { get; set; } //STORE Id
        public long GMID { get; set; }//MATRIX Id
    }

}
