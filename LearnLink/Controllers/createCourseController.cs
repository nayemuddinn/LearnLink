using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class CreateCourseController : Controller
    {
        public ActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCourse(Course course)
        {
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();

                    string query = "INSERT INTO courses (CourseName, CourseFee, CourseDescription, CoursePrerequisite, TeacherID) VALUES (@CourseName, @CourseFee, @CourseDescription, @CoursePrerequisite, @TeacherID)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseName", course.CourseName);
                        cmd.Parameters.AddWithValue("@CourseFee", course.CourseFee);
                        cmd.Parameters.AddWithValue("@CourseDescription", course.CourseDescription);
                        cmd.Parameters.AddWithValue("@CoursePrerequisite", course.CoursePrerequisite);
                        cmd.Parameters.AddWithValue("@TeacherID", Session["UserID"]);

                        cmd.ExecuteNonQuery();

                        Response.Write("<script>alert('Course created successfully!');</script>");
                      
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred. Please try again.');</script>");
                
                }
            }
            return View();
        }
    }
}
