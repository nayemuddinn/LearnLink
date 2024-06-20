using System.Collections.Generic;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class TeacherDashboardController : Controller
    {
        public ActionResult Dashboard()
        {
            // Dummy data for demonstration
            ViewBag.Courses = new List<string> { "Mathematics", "Physics", "Computer Science" };
            ViewBag.Tasks = new List<string> { "Grade Math Assignment 1", "Prepare Physics Lab", "Review CS Project" };
            ViewBag.Announcements = new List<string> { "Meeting on Monday", "New course material available", "Project deadlines updated" };
            return View();
        }
    }
}
