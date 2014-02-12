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
            CalendarWeekDate calendarWeek=new CalendarWeekDate();
            calendarWeek.BeginDate = DateHelper.GetWeekBeginDate(selcTime, WeekDay.Sunday);
            calendarWeek.EndDate = calendarWeek.BeginDate.AddDays(6);
            List<WeekDate> weeklList=new List<WeekDate>();
            for (int i = 0; i <= 6; i++)
            {
                weeklList.Add(new WeekDate()
                {
                    DateString = calendarWeek.BeginDate.AddDays(i).Date.ToString(),
                    WeekString = GetWeekName(i)
                });
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

        [HttpPost]
        public ActionResult CalendarForm(CalendarInfo calendarEntity)
        {
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
                string name = Session["user"].ToString();
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
