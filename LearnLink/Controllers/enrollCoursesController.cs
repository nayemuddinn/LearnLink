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
                string query = @"
                    SELECT TOP 5 c.CourseID, c.CourseName,c.courseFee, c.TeacherID, t.Name
                    FROM Courses c
                    JOIN Teacher t ON c.TeacherID = t.UserID
                    ORDER BY c.CourseID DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    topCourses.Add(new Course
                    {
                        CourseID = (int)reader["CourseID"],
                        CourseName = (string)reader["CourseName"],
                        CourseFee = (int)reader["CourseFee"],
                        TeacherID = (int)reader["TeacherID"],
                        TeacherName = (string)reader["Name"]

                    });
                }
            }

            return View(topCourses);

        }
    }
}