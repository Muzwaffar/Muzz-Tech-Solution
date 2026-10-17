using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Transaction
{
    public class ChatBaseDTO
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string TimeStamp { get; set; } = string.Empty;
        public bool IsSender { get; set; } = false;
    }
    public class ChatSaveDTO
    {
        public long IncidentId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
