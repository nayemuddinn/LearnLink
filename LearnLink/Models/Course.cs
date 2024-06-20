using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class Course
    {
        public string Title { get; set; }
        public string Instructor { get; set; }
        public int Progress { get; set; } // Percentage of completion
    }
}