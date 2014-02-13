using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            TempData["err"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserInfo user)
        {
            using (CalendarDBEntities calEntityEntities = new CalendarDBEntities())
            {
                if (user!=null)
                {
                     IQueryable<UserInfo> userInfos = calEntityEntities.UserInfo.Where(
                        x => x.UserName == user.UserName && x.UserPassword == user.UserPassword);
                    
                    if (userInfos.Any())
                    {
                        Session["user"] = userInfos.First();
                        return RedirectToAction("CalendarNavigation", "Home");
                    }
                    else
                    {
                        TempData["err"] = "<script>alert('Your account or password is not valid')</script>";
                    }
                }
                return View();
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserInfo user)
        {
            using (CalendarDBEntities calEntityEntities=new CalendarDBEntities())
            {
                if (user!=null)
                {
                    calEntityEntities.UserInfo.Add(user);
                    calEntityEntities.SaveChanges();
                }
                return View();
            }
             
        }

    }
}
