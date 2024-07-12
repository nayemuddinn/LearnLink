using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class enrollCoursesController : Controller
    {

        [HttpPost]
        public ActionResult EnrollCourse(int Id)
        {
           /* using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "INSERT INTO EnrollmentRequests (StudentID, CourseID,TeacherID, RequestDate, Status) VALUES (@StudentID, @CourseID,@TeacherID, @RequestDate, @Status)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", Session["UserID"]);
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                cmd.Parameters.AddWithValue("@TeacherID", teacherId);
                cmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Status", "Requested");
                conn.Open();
                cmd.ExecuteNonQuery();*/

                Response.Write("<script>alert('Course Enrollment Request Sent ');</script>");


                return RedirectToAction("StudentCourseDetails", "AllCourse");
            //}

        }
    }
}
