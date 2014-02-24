using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using MvcApplication1.Models.Common;
using MvcApplication1.Models.Enum;
using MvcApplication1.Models.Interface;

namespace MvcApplication1.Controllers
{
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/

        private readonly ICalendarRepository _calendarRepository=new CalendarRepository();

        public ActionResult DeptCalendar()
        {
            return View(GetWeekCalendar(DateTime.Now));
        }
        
        public CalendarWeekDate GetWeekCalendar(DateTime selcTime)
        {
             return _calendarRepository.GetWeekCalendar(selcTime);
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

        public ActionResult TreeViewShow(string selectNodes)
        {
            ViewBag.SelectNodes = selectNodes;
            return View();
        }

        public ActionResult PersonalCalendar()
        {
            return View();
        }
    }
}
