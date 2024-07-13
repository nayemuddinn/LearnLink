using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class EnrolledStudentCourse
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string Institution { get; set; }
        public string Phone { get; set; } 
    }
}