using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public string CoursePrerequisite { get; set; }
        public int CourseFee { get; set; }
        public int TeacherID { get; set; }
        public DateTime CourseCreateDate { get; set; }
    }
}