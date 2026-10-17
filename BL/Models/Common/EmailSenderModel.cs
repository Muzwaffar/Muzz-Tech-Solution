using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public class EmailSenderModel<T>
    {
        public string Subject { get; set; } = string.Empty;
        public string ReplyTo { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public List<string> Body { get; set; } = new List<string>();
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public List<string> To { get; set; } = new List<string>();
        public T Signature { get; set; }
        public List<T> Attachment { get; set; }
        public List<string> Users { get; set; } = new List<string>();
        public bool IsTest { get; set; } = true;
    }
}
