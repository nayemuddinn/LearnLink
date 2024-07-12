using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class enrollCoursesController : Controller
    {
        // GET: enrolledCourses
        public ActionResult EnrollCourses()
        {
            List<Course>topCourses = new List<Course>();
      
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "SELECT TOP 5 * FROM Courses ORDER BY CourseID DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    topCourses.Add(new Course
                    {
                        CourseID = (int)reader["CourseID"],
                        CourseName = (string)reader["CourseName"],
                        TeacherID = (int)reader["TeacherID"]
                       
                    });
                }
            }

            return View(topCourses);

        }
    }
}