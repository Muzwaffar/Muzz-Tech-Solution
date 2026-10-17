using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public class DateRangeDTO
    {
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;

    }
    public class DateRangeModuleDTO
    {
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;
        public ActivityModule ActivityModule { get; set; } = ActivityModule.General;

    }

    public class DateRangeFormDTO
    {
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;
        public string Form { get; set; } = string.Empty;
        public string ListingName { get; set; } = string.Empty;
    }
}
