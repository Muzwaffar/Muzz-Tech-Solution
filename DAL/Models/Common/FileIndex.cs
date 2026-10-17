using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Common
{
    public class FileIndex : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FileId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string Path { get; set; }
        public string ConcatKeywords { get; set; }
    }
}
