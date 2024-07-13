using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class enrollStudentsController : Controller
    {
        // GET: enrollStudents
        public ActionResult enrollNewStudents()
        {
            return View();
        }

        public ActionResult ViewEnrolledStudents()
        {
            return View();
        }
    }
}