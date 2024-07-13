using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class StudentEnrolledCoursesController : Controller
    {
        // GET: StudentEnrolledCourses
        public ActionResult ViewEnrolledCourses()
        {
            return View();
        }
    }
}