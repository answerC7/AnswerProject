using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using MvcApplication1.Models.Common;
using MvcApplication1.Models.Enum;

namespace MvcApplication1.Controllers
{
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/

        public ActionResult DeptCalendar()
        {
           
            return View(GetWeekCalendar(DateTime.Now));
        }
        
        public CalendarWeekDate GetWeekCalendar(DateTime selcTime)
        {
            var calendarWeek=new CalendarWeekDate();
            calendarWeek.BeginDate = DateHelper.GetWeekBeginDate(selcTime, WeekDay.Sunday);
            calendarWeek.EndDate = calendarWeek.BeginDate.AddDays(6);
            var weeklList=new List<WeekDate>();
            //forenoon calendar
            var forenoonCalendar = new List<CalendarInfo>();
            //afternoon calendar
            var afternoonCalendar = new List<CalendarInfo>();
            var currentUser = (UserInfo)Session["user"];
            for (int i = 0; i <= 6; i++)
            {
                weeklList.Add(new WeekDate()
                {
                    DateString = calendarWeek.BeginDate.AddDays(i).Date.ToString(),
                    WeekString = GetWeekName(i)
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


        public string GetWeekName(int weekIndex)
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

        public ActionResult CalendarForm()
        {
            ViewBag.Title = "Add New Calendar";
            return View();
        }

        public ActionResult CalendarEdit(int calendarId)
        {
            CalendarInfo calendarObj=new CalendarInfo();
            using (var calendarEntity = new CalendarDBEntities())
            {
                 //calendarObj =calendarEntity.CalendarInfo.FirstOrDefault(x => x.ID == calendarId);

                 //var joinList = calendarEntity.JoinInfo.Where(x => x.CalendarId == calendarId).ToList();

                var calendarList = (from cf in calendarEntity.CalendarInfo
                    join jf in calendarEntity.JoinInfo on cf.ID equals jf.CalendarId
                    where cf.ID == calendarId
                    select new
                    {
                        cf.ID,
                        cf.Title,
                        jf.JoinerAccount,
                        cf.StartTime,
                        cf.EndTime,
                        cf.Location

                    }
                    );

                var calendarData = (from aa in calendarList.ToList()
                                select new CalendarInfo
                                {
                                    ID = aa.ID,
                                    Title = aa.Title,
                                    JoinAccounts = aa.JoinerAccount,
                                    StartTime = aa.StartTime,
                                    EndTime = aa.EndTime,
                                    Location = aa.Location
                                }).ToList();


                calendarObj = calendarData.GroupJoin(calendarData, a => a.ID, b => b.ID,
                       (a, b) => new CalendarInfo
                       {
                           ID = a.ID,
                           Title = a.Title,
                           JoinAccounts = b.Select(p => p.JoinAccounts).Aggregate((c, d) => c + ',' + d),
                           StartTime = a.StartTime,
                           EndTime = a.EndTime,
                           Location = a.Location
                       }
                       ).GroupBy(p => p.ID).Select(g => g.First()).ToList().First();

            }

            return View("CalendarForm", calendarObj);
        }

        [HttpPost]
        public ActionResult CalendarForm(CalendarInfo calendarEntity)
        {
            calendarEntity.CalendarCode = ((UserInfo) Session["user"]).UserDeptCode;

            calendarEntity.CalendarType = 0;
            //joiner arrary
            string[] accountsArr = calendarEntity.JoinAccounts.Split(',');
            
            using (CalendarDBEntities contextEntities=new CalendarDBEntities())
            {              
                using (TransactionScope transaction = new TransactionScope())
                {
                    contextEntities.CalendarInfo.Add(calendarEntity);
                    contextEntities.SaveChanges();

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

            return View();
        }

        /// <summary>
        /// 获取json数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTreeData()
        {         
            using (CalendarDBEntities calEntityEntities = new CalendarDBEntities())
            {
                string name = ((UserInfo)Session["user"]).UserName;
                 var list = (from uf in calEntityEntities.UserInfo
                             where uf.UserName != name
                    select new
                    {
                        id=uf.Uid,
                        pid=0,
                        name=uf.UserName
                    });

                 return Json(list.ToList(), JsonRequestBehavior.AllowGet);         
            }

            
        }

        public ActionResult TreeViewShow()
        {
            return View();
        }

        public ActionResult PersonalCalendar()
        {
            return View();
        }
    }
}
