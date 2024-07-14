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

                 string query = @"SELECT q.QuizID, q.CourseID, q.TeacherID, q.Title, q.Description, q.CreationDate, q.CourseName, q.Duration, q.Status
                 FROM Quiz q INNER JOIN Enrollment e ON q.CourseID = e.CourseID
                 WHERE e.StudentID = @StudentID";
                

                    using (SqlCommand cmd = new SqlCommand(query, conn))
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
                                    TeacherID = reader["TeacherID"] != DBNull.Value ? Convert.ToInt32(reader["TeacherID"]) : 0,
                                    Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : "No Title",
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "No Description",
                                    CreationDate = reader["CreationDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreationDate"]) : DateTime.MinValue,
                                    CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "No Course Name",
                                    Duration = reader["Duration"] != DBNull.Value ? Convert.ToInt32(reader["Duration"]) : 0,
                                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "No Status"
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred while fetching quizzes. Please try again."+ex.Message+"');</script>");
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

     
            return View(questions);
        }


    }
}