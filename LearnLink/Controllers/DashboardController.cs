using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            // Create sample data for the dashboard
            var model = new DashboardViewModel
            {
                TotalCourses = 10,
                TotalStudents = 150,
                CompletedCourses = 5,
                UpcomingEvents = new List<Event>
                {
                    new Event { Title = "Web Development Workshop", Date = DateTime.Now.AddDays(5), Description = "Learn the basics of web development." },
                    new Event { Title = "Advanced C# Seminar", Date = DateTime.Now.AddDays(10), Description = "Deep dive into advanced C# topics." }
                },
                RecentActivities = new List<Activity>
                {
                    new Activity { Description = "John Doe completed 'Introduction to Programming'", Date = DateTime.Now.AddDays(-1) },
                    new Activity { Description = "Jane Smith enrolled in 'Data Science 101'", Date = DateTime.Now.AddDays(-2) }
                },
                Courses = new List<Course>
                {
                    new Course { Title = "Introduction to Programming", Instructor = "Prof. John Doe", Progress = 100 },
                    new Course { Title = "Data Science 101", Instructor = "Dr. Jane Smith", Progress = 40 }
                }
            };

            return View(model);
        }
    }
}