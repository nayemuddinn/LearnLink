using System.Collections.Generic;

namespace LearnLink.Models
{
    public class QuizAnalytics
    {
        public Quiz Quiz { get; set; }

        public List<QuizEvaluation> Evaluations { get; set; }

        public int TotalSubmissions { get; set; }

        public int TotalQuestions { get; set; }

        public double AverageScore { get; set; }

        public int HighestScore { get; set; }

        public int LowestScore { get; set; }
    }
}