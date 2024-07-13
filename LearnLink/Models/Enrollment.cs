using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentInstitution { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}