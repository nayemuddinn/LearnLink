using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class QuizQuestion
    {
        public int QuestionID { get; set; }
        public int QuizID { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public char CorrectOption { get; set; }
    }
}