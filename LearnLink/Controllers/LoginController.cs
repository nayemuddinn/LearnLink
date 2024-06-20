using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;


namespace LearnLink.Controllers
{
    
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (user.Email.Equals("abc@gmail.com") && user.Password.Equals("1234"))
            {
                Session["User"] = user.Name;
                return RedirectToAction("Dashboard", "Dashboard");

            }
            else
                ViewBag.Log = "Login Failed";

            return View();
        }
    }
}


