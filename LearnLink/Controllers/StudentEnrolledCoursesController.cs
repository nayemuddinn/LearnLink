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
    public class StudentEnrolledCoursesController : Controller
    {

        public ActionResult ViewEnrolledCourses()
        {
            List<Course> courses = new List<Course>();

            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = @"
            SELECT c.CourseID, c.CourseName, t.Name
            FROM Courses c
            JOIN Enrollment e ON c.CourseID = e.CourseID
            JOIN Teacher t ON c.TeacherID = t.userID
            WHERE e.StudentID = @StudentID AND e.Status = 'Accepted'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", Session["UserID"]);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                CourseID = reader["CourseID"] != DBNull.Value ? Convert.ToInt32(reader["CourseID"]) : 0,
                                CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "No Name",
                                TeacherName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "No Teacher"
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(courses);
        }

        public ActionResult Unenroll( int courseid)
        {
            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = "DELETE FROM enrollment WHERE StudentID = @StudentID AND CourseID = @CourseID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@CourseID", courseid);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return RedirectToAction("ViewEnrolledStudents");
        }


    }
}