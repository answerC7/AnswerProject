using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication1.Models.Enum;

namespace MvcApplication1.Models
{
    public class WeekDate
    {
        public string WeekString { get; set; }

        public string DateString { get; set; }

        public List<CalendarInfo> Forenoon { get; set; }

        public List<CalendarInfo> Afternoon { get; set; }

    }
}