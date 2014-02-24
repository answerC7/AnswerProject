using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication1.Models.Enum;

namespace MvcApplication1.Models.Common
{
    public class DateHelper
    {
        public static DateTime GetWeekBeginDate(DateTime selectTime,WeekDay weekstartDay)
        {
             int startIndex = weekstartDay.GetHashCode();
             int offset = startIndex - selectTime.DayOfWeek.GetHashCode();
            if (offset > 0)
                offset = offset - 7;

            return selectTime.AddDays(offset).Date;
        }

        public static string GetWeekName(int weekIndex)
        {
            switch (weekIndex)
            {
                case 0:
                    return "Sunday";
                case 1:
                    return "Monday";
                case 2:
                    return "Tuesday";
                case 3:
                    return "Wednesday";
                case 4:
                    return "Thursday";
                case 5:
                    return "Friday";
                case 6:
                    return "Saturday";
                default:
                    return null;
            }
        }
    }
}