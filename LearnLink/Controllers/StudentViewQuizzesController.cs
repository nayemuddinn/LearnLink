using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class StudentViewQuizzesController : Controller
    {


        public ActionResult ViewQuizzes()
        {
            List<Quiz> quizzes = new List<Quiz>();

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();

                    string queryQuizzes = "SELECT q.QuizID, q.CourseID, q.CourseName, q.TeacherID, q.Title, q.Duration, q.Description, q.CreationDate, q.Status, CASE WHEN qe.Score IS NOT NULL THEN qe.Score ELSE -1 " +
                        "END AS Score FROM Quiz q INNER JOIN Enrollment e ON q.CourseID = e.CourseID LEFT JOIN QuizEvaluation qe ON q.QuizID = qe.QuizID AND qe.StudentID = @StudentID WHERE e.StudentID = @StudentID";


                    using (SqlCommand cmd = new SqlCommand(queryQuizzes, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", (int)Session["UserID"]);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quizzes.Add(new Quiz
                                {
                                    QuizID = reader["QuizID"] != DBNull.Value ? Convert.ToInt32(reader["QuizID"]) : 0,
                                    CourseID = reader["CourseID"] != DBNull.Value ? Convert.ToInt32(reader["CourseID"]) : 0,
                                    CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "No Course Name",
                                    TeacherID = reader["TeacherID"] != DBNull.Value ? Convert.ToInt32(reader["TeacherID"]) : 0,
                                    Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : "No Title",
                                    Duration = reader["Duration"] != DBNull.Value ? Convert.ToInt32(reader["Duration"]) : 0,
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "No Description",
                                    CreationDate = reader["CreationDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreationDate"]) : DateTime.MinValue,
                                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "No Status",
                                    Score = reader["Score"] != DBNull.Value ? Convert.ToInt32(reader["Score"]) : -1
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred while fetching quizzes. Please try again. " + ex.Message + "');</script>");
                }
            }

            return View(quizzes);
        }




        public ActionResult StartQuiz(int id)
        {
            Quiz quiz = null;
            List<QuizQuestion> questions = new List<QuizQuestion>();
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();
                string queryQuiz = "SELECT * FROM Quiz WHERE QuizID = @QuizID";
                using (SqlCommand cmd = new SqlCommand(queryQuiz, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quiz = new Quiz
                            {
                                QuizID = (int)reader["QuizID"],
                                Title = reader["Title"].ToString(),
                                Duration = (int)reader["Duration"]
                            };
                        }
                    }
                }

                string queryQuestions = "SELECT * FROM QuizQuestions WHERE QuizID = @QuizID";
                using (SqlCommand cmd = new SqlCommand(queryQuestions, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new QuizQuestion
                            {
                                QuizID= (int)reader["QuizID"],
                                QuestionID = (int)reader["QuestionID"],
                                Question = reader["Question"].ToString(),
                                OptionA = reader["OptionA"].ToString(),
                                OptionB = reader["OptionB"].ToString(),
                                OptionC = reader["OptionC"].ToString(),
                                OptionD = reader["OptionD"].ToString(),
                                CorrectOption = reader["CorrectOption"].ToString()
                            });
                        }
                    }
                }
            }

            Session["QuizStartTime"] = DateTime.Now;
            Session["QuizDuration"] = quiz.Duration;
            Session["QuizID"] = quiz.QuizID;


            return View(questions);
        }


        [HttpPost]
        public ActionResult SubmitQuiz(FormCollection form)
        {
            int quizID = (int)Session["QuizID"];
            DateTime quizStartTime = (DateTime)Session["QuizStartTime"];
            int quizDuration = (int)Session["QuizDuration"];

            if ((DateTime.Now - quizStartTime).TotalMinutes > quizDuration)
            {
                Session["TLE"]= "Quiz time exceeded. Submission not accepted";
                RedirectToAction("Dashboard", "StudentDashboard");
            }

            int score = 0;
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();

                Dictionary<int, string> correctAnswers = new Dictionary<int, string>();
                string queryCorrectAnswers = "SELECT QuestionID, CorrectOption FROM QuizQuestions WHERE QuizID = @QuizID";
                using (SqlCommand cmd = new SqlCommand(queryCorrectAnswers, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            correctAnswers.Add((int)reader["QuestionID"], reader["CorrectOption"].ToString());
                        }
                    }
                }

       
                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("question_"))
                    {
                        int questionID = int.Parse(key.Split('_')[1]);
                        string selectedOption = form[key];

                        if (correctAnswers.ContainsKey(questionID) && correctAnswers[questionID] == selectedOption)
                        {
                            score++;
                        }
                    }
                }

   
                string query = "INSERT INTO QuizEvaluation (StudentID, QuizID, Score) VALUES (@StudentID, @QuizID, @Score)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", (int)Session["UserID"]);
                    cmd.Parameters.AddWithValue("@QuizID", quizID);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.ExecuteNonQuery();
                }
            }

            Response.Write("<script>alert('Quiz submitted successfully!');</script>");
            return RedirectToAction("Dashboard", "StudentDashboard");
        }






    }
}