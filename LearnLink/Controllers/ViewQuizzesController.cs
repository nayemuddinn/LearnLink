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
    public class ViewQuizzesController : Controller
    {

        public ActionResult ViewQuizzes()
        {
  
            List<Quiz> quizzes = new List<Quiz>();

            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = "SELECT QuizID, CourseID, TeacherID,CourseName,Title, Description, CreationDate FROM Quiz WHERE TeacherID = @TeacherID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", Session["UserID"]);
                    con.Open();
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
                                CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "NoCourseName"
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(quizzes); 
        }
    }
}