using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class CalendarWeekDate
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<WeekDate>DateList{get; set;}

    }
}