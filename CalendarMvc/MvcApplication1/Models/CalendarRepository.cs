using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using MvcApplication1.Models.Common;
using MvcApplication1.Models.Enum;
using MvcApplication1.Models.Interface;

namespace MvcApplication1.Models
{
    public class CalendarRepository : ICalendarRepository
    {
        public CalendarWeekDate GetWeekCalendar(DateTime selcTime)
        {
            var calendarWeek = new CalendarWeekDate();
            calendarWeek.BeginDate = DateHelper.GetWeekBeginDate(selcTime, WeekDay.Sunday);
            calendarWeek.EndDate = calendarWeek.BeginDate.AddDays(6);
            var weeklList = new List<WeekDate>();
            //forenoon calendar
            var forenoonCalendar = new List<CalendarInfo>();
            //afternoon calendar
            var afternoonCalendar = new List<CalendarInfo>();
            //var currentUser = (UserInfo)Session["user"];
            for (int i = 0; i <= 6; i++)
            {
                weeklList.Add(new WeekDate()
                {
                    DateString = calendarWeek.BeginDate.AddDays(i).Date.ToString(),
                    WeekString = DateHelper.GetWeekName(i)
                });

                //get weekday calendar             
                using (var calendarEntity = new CalendarDBEntities())
                {
                    var startTime = Convert.ToDateTime(weeklList[i].DateString);
                    var midTime = startTime.AddHours(12);
                    var endTime = startTime.AddHours(24);
                    var forenoonList = (from cf in calendarEntity.CalendarInfo
                                        join jf in calendarEntity.JoinInfo on cf.ID equals jf.CalendarId
                                        where cf.StartTime >= startTime && cf.StartTime <= midTime
                                        orderby cf.StartTime ascending
                                        select new
                                        {
                                            cf.ID,
                                            cf.Title,
                                            jf.JoinerAccount,
                                            cf.StartTime,
                                            cf.EndTime,
                                            cf.Location
                                        });

                    var afternoonList = (from cf in calendarEntity.CalendarInfo
                                         join jf in calendarEntity.JoinInfo on cf.ID equals jf.CalendarId
                                         where cf.StartTime >= midTime && cf.StartTime <= endTime
                                         orderby cf.StartTime
                                         select new
                                         {
                                             cf.ID,
                                             cf.Title,
                                             jf.JoinerAccount,
                                             cf.StartTime,
                                             cf.EndTime,
                                             cf.Location
                                         });

                    var foreList = (from aa in forenoonList.ToList()
                                    select new CalendarInfo
                                    {
                                        ID = aa.ID,
                                        Title = aa.Title,
                                        JoinAccounts = aa.JoinerAccount,
                                        StartTime = aa.StartTime,
                                        EndTime = aa.EndTime,
                                        Location = aa.Location
                                    }).ToList();

                    var afterList = (from aa in afternoonList.ToList()
                                     select new CalendarInfo
                                     {
                                         ID = aa.ID,
                                         Title = aa.Title,
                                         JoinAccounts = aa.JoinerAccount,
                                         StartTime = aa.StartTime,
                                         EndTime = aa.EndTime,
                                         Location = aa.Location
                                     }).ToList();

                    //Aggregate joinaccount
                    weeklList[i].Forenoon = foreList.GroupJoin(foreList, a => a.ID, b => b.ID,
                         (a, b) => new CalendarInfo
                         {
                             ID = a.ID,
                             Title = a.Title,
                             JoinAccounts = b.Select(p => p.JoinAccounts).Aggregate((c, d) => c + ',' + d),
                             StartTime = a.StartTime,
                             EndTime = a.EndTime,
                             Location = a.Location
                         }
                         ).GroupBy(p => p.ID).Select(g => g.First()).ToList();

                    weeklList[i].Afternoon = afterList.GroupJoin(afterList, a => a.ID, b => b.ID,
                       (a, b) => new CalendarInfo
                       {
                           ID = a.ID,
                           Title = a.Title,
                           JoinAccounts = b.Select(p => p.JoinAccounts).Aggregate((c, d) => c + ',' + d),
                           StartTime = a.StartTime,
                           EndTime = a.EndTime,
                           Location = a.Location
                       }
                       ).GroupBy(p => p.ID).Select(g => g.First()).ToList();

                }

            }
            calendarWeek.DateList = weeklList;

            return calendarWeek;
        }

        public int AddCalendar(CalendarInfo calendarEntity)
        {
            int calendarId = 0;
            //joiner arrary
            string[] accountsArr = calendarEntity.JoinAccounts.Split(',');

            using (CalendarDBEntities contextEntities = new CalendarDBEntities())
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    CalendarInfo calendarInfo = contextEntities.CalendarInfo.Add(calendarEntity);
                    contextEntities.SaveChanges();
                    calendarId = calendarInfo.ID;
                    foreach (var account in accountsArr)
                    {
                        contextEntities.JoinInfo.Add(new JoinInfo()
                        {
                            CalendarId = calendarEntity.ID,
                            JoinerAccount = account
                        });

                        contextEntities.SaveChanges();
                    }
                   
                    transaction.Complete();
                }
            }

            return calendarId;
        }

        public int UpdateCalendar(int calendarId)
        {
            throw new NotImplementedException();
        }
    }
}