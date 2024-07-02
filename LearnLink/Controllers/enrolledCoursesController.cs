using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class enrolledCoursesController : Controller
    {
        // GET: enrolledCourses
        public ActionResult enrolledCourses()
        {
            return View();
        }
    }
}