﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class Quiz
    {
        public int QuizID { get; set; }
        public int CourseID { get; set; }
        public int TeacherID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }

}