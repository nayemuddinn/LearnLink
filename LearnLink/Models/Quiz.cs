using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class Quiz
    {
        public int QuizID { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int TeacherID { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public int Score { get; set; }
    }

}