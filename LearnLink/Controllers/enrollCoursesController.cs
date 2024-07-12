using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class enrollCoursesController : Controller
    {
        // GET: enrolledCourses
        public ActionResult EnrollCourses()
        {
            return View();

        }
    }
}