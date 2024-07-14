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
    public class TeacherViewQuizzesController : Controller
    {

        public ActionResult ViewQuizzes()
        {
  
            List<Quiz> quizzes = new List<Quiz>();

            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = "SELECT QuizID, CourseID, TeacherID, CourseName, Title, Description, CreationDate, Status FROM Quiz WHERE TeacherID = @TeacherID ORDER BY QuizID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", (int)Session["UserID"]);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            quizzes.Add(new Quiz
                            {
                                QuizID = reader["QuizID"] != DBNull.Value ? Convert.ToInt32(reader["QuizID"]) : 0,
                                CourseID = reader["CourseID"] != DBNull.Value ? Convert.ToInt32(reader["CourseID"]) : 0,
                                Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : "No Title",
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "No Description",
                                CreationDate = reader["CreationDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreationDate"]) : DateTime.MinValue,
                                CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "NoCourseName",
                                Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "NotStarted"
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(quizzes); 
        }
        public ActionResult DeleteQuiz(int id)
        {
            string connectionString = DBconnection.connStr;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string deleteQuizEvaluationsQuery = "DELETE FROM QuizEvaluation WHERE QuizID = @QuizID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuizEvaluationsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", id);
                        cmd.ExecuteNonQuery();
                    }

                    string deleteQuizQuestionsQuery = "DELETE FROM QuizQuestions WHERE QuizID = @QuizID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuizQuestionsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", id);
                        cmd.ExecuteNonQuery();
                    }

                    string deleteQuizQuery = "DELETE FROM Quiz WHERE QuizID = @QuizID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuizQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", id);
                        cmd.ExecuteNonQuery();
                    }

                    Response.Write("<script>alert('Quiz deleted successfully!');</script>");
                }
                catch (Exception ex)
                {
                    Response.Write($"<script>alert('An error occurred while deleting the quiz: {ex.Message}');</script>");
                }
            }

            return RedirectToAction("ViewQuizzes");
        }




        public ActionResult StartQuiz(int id)
        {
           
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Quiz SET Status = @Status WHERE QuizID = @QuizID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", "Started");
                        cmd.Parameters.AddWithValue("@QuizID", id);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return HttpNotFound("Quiz not found or already started.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred while Starting the quiz. Please try again');</script>");
                }
            }

            return RedirectToAction("ViewQuizzes");
        }




    }
}