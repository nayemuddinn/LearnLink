using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class QuizEvaluation
    {
        public int SubmitID { get; set; } 
        public int StudentID { get; set; }
        public int StudentName { get; set; }
        public int QuizID { get; set; }
        public int Score { get; set; }
        public DateTime SubmissionTime { get; set; }
        public  string Feedback { get; set; }
    }
}