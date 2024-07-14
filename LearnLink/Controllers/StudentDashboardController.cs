using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class StudentDashboardController : Controller
    {
        public ActionResult Dashboard()
        {
         
            return View();
        }
    }
}