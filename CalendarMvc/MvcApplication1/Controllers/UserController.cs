using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using MvcApplication1.Models.Interface;

namespace MvcApplication1.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository = new UserRepository();

        public ActionResult Login()
        {
            TempData["err"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserInfo user)
        {
            UserInfo userInfo = _userRepository.Login(user);
            if (userInfo != null)
            {
                Session["user"] = userInfo;
                return RedirectToAction("CalendarNavigation", "Home");
            }
            TempData["err"] = "<script>alert('Your account or password is not valid')</script>";
            return View();          
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserInfo user)
        {
            UserInfo userModel = _userRepository.Register(user);
            if (userModel != null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

    }
}
