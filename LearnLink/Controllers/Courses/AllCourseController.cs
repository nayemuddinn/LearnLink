using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers.Courses
{
    public class AllCourseController : Controller
    {

        public ActionResult AllCourse(string searchTerm)
        {
            List<Course> courses = new List<Course>();

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
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
                    courses.Add(new Course
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
            return View(courses);

        }



        public ActionResult StudentCourseDetails(int cId)
        {


            Course courseDetails = new Course();

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();
                string query = @"
                SELECT c.CourseID, c.CourseName, c.CourseDescription, c.CoursePrerequisite, c.CourseFee, c.TeacherID,t.Name
                FROM Courses c
                JOIN Teacher t ON c.TeacherID = t.UserID
                WHERE c.CourseID = @CourseID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CourseID", cId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    courseDetails.CourseID = (int)reader["CourseID"];
                    courseDetails.CourseName = (string)reader["CourseName"];
                    courseDetails.CourseDescription = (string)reader["CourseDescription"];
                    courseDetails.CoursePrerequisite = (string)reader["CoursePrerequisite"];
                    courseDetails.CourseFee = (int)reader["CourseFee"];
                    courseDetails.TeacherID = (int)reader["TeacherID"];
                    courseDetails.TeacherName = (string)reader["Name"];
                }
            }

            string courseStatus = getCourseStatus(courseDetails.CourseID);
            ViewBag.EnrollmentStatus = courseStatus;

            return View(courseDetails);

        }

        public string getCourseStatus(int courseId)
        {
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string checkQuery = "SELECT Status FROM Enrollment WHERE StudentID = @StudentID AND CourseID = @CourseID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@StudentID", Session["UserID"]);
                checkCmd.Parameters.AddWithValue("@CourseID", courseId);

                conn.Open();
                var status = checkCmd.ExecuteScalar();
                return status != null ? status.ToString() : null;
            }
        }


    }
}
