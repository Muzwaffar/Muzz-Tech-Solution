using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Common
{
    public class FileProperties : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FileIndexId { get; set; }
        public long RecordId { get; set; }//Id of data in DbTable
        public ParentModule ParentModule { get; set; }// Research/Extension.....
        public ResearchType Module { get; set; }//Crop/Livestock.....
        public ResearchSubModule SubModule { get; set; }//POW/LandRequirement
        public int TableId { get; set; }
    }
}
