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
    public class AllCourseController : Controller
    {
      
        public ActionResult AllCourse(string searchTerm)
        {
            List<Course> topCourses = new List<Course>();

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                // Check if search term is null or empty
                string query = searchTerm == null || searchTerm.Trim() == ""
                    ? @"
                        SELECT TOP 5 c.CourseID, c.CourseName,c.CourseFee, c.TeacherID, t.Name
                        FROM Courses c
                        JOIN teacher t ON c.TeacherID = t.UserID
                        ORDER BY c.CourseID DESC"
                    : @"
                        SELECT TOP 5 c.CourseID, c.CourseName, c.CourseFee,c.TeacherID, t.Name
                        FROM Courses c
                        JOIN Teacher t ON c.TeacherID = t.UserID
                        WHERE c.CourseName LIKE '%' + @SearchTerm + '%' 
                        OR t.Name LIKE '%' + @SearchTerm + '%' 
                        OR c.CourseID LIKE '%' + @SearchTerm + '%'
                        ORDER BY c.CourseID DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                }
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

            ViewBag.SearchTerm = searchTerm;
            return View(topCourses);

        }
    }
}