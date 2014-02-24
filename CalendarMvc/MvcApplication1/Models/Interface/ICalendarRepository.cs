using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication1.Models.Interface
{
    public interface ICalendarRepository
    {
        /// <summary>
        /// get calendar data
        /// </summary>
        /// <param name="selcTime">select time</param>
        /// <returns></returns>
        CalendarWeekDate GetWeekCalendar(DateTime selcTime);

        /// <summary>
        /// add new calendar
        /// </summary>
        /// <param name="calendarEntity">calendar Data</param>
        /// <returns>calendarId</returns>
        int AddCalendar(CalendarInfo calendarEntity);

        /// <summary>
        /// update calendar by calendarId
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        int UpdateCalendar(int calendarId);

    }
}
