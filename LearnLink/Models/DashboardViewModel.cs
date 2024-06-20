using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class DashboardViewModel
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int CompletedCourses { get; set; }
        public List<Event> UpcomingEvents { get; set; }
        public List<Activity> RecentActivities { get; set; }
        public List<Course> Courses { get; set; }
    }
}