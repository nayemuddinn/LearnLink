using LearnLink.Content;
using LearnLink.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class CreateQuizController : Controller
    {
      
        public ActionResult CreateQuiz()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateQuiz(Quiz quiz)
        {
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();

                    string query = "INSERT INTO Quiz (CourseID, TeacherID, Title, Description, CreationDate) VALUES (@CourseID, @TeacherID, @Title, @Description, @CreationDate)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseID", quiz.CourseID);
                        cmd.Parameters.AddWithValue("@TeacherID", Session["UserID"]);
                        cmd.Parameters.AddWithValue("@Title", quiz.Title);
                        cmd.Parameters.AddWithValue("@Description", quiz.Description);
                        cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);

                        cmd.ExecuteNonQuery();

                        Response.Write("<script>alert('Quiz created successfully!');</script>");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred. Please try again.');</script>");
                }
            }
            return View();
        }


        public ActionResult UploadQuiz()
        {
            return View();
        }
    }
}