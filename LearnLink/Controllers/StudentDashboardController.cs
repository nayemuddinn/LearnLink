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
            ViewBag.Courses = new List<string> { "Mathematics", "Physics", "Computer Science" };
            ViewBag.Assignments = new List<string> { "Math Assignment 1", "Physics Lab Report", "CS Project" };
            ViewBag.Announcements = new List<string> { "Holiday on Friday", "Guest Lecture on AI", "Midterm Schedule" };
            return View();
        }
    }
}