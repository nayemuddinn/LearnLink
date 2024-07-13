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
    public class enrollStudentsController : Controller
    {
        // GET: enrollStudents
        public ActionResult enrollStudents()
        {
            return View();
        }
        public ActionResult enrollNewStudents()
        {
            List<Enrollment> requests = new List<Enrollment>();

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = @"
                    SELECT r.EnrollmentID, c.CourseID, c.CourseName, r.StudentID, s.Name, s.Institution, r.Status
                    FROM Enrollment r
                    INNER JOIN Courses c ON r.CourseID = c.CourseID
                    INNER JOIN Student s ON r.StudentID = s.UserID
                    WHERE r.TeacherID = @TeacherID AND r.Status = 'Requested'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", Session["UserID"]);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    requests.Add(new Enrollment
                    {
                        EnrollmentID = (int)reader["EnrollmentID"],
                        CourseID = (int)reader["CourseID"],
                        CourseName = (string)reader["CourseName"],
                        StudentID = (int)reader["StudentID"],
                        StudentName = (string)reader["Name"],
                        StudentInstitution = (string)reader["Institution"],
                        Status = (string)reader["Status"]
                    });
                }
            }

            return View(requests);

        }

        public ActionResult UpdateRequest(int enrollmentID, string actionType)
        {
            string status = "";

            if (actionType.ToLower() == "accept")
            {
                status = "Accepted";
            }
            else if (actionType.ToLower() == "reject")
            {
                status = "Rejected";
            }
          

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "UPDATE Enrollment SET Status = @Status WHERE enrollmentID = @enrollmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@enrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }


            return RedirectToAction("enrollNewStudents", "enrollStudents");
        }



        public ActionResult ViewEnrolledStudents(int courseID)
        {
            List<User> enrolledStudents = new List<User>();

            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = @"
            SELECT s.UserID, s.Name, s.Institution, s.Phone
            FROM student s
            JOIN Enrollment e ON s.UserID = e.StudentID
            WHERE e.CourseID = @CourseID AND e.Status = 'Accepted'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            enrolledStudents.Add(new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Name = reader["Name"].ToString(),
                                Institution = reader["Institution"].ToString(),
                                Phone = reader["Phone"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            ViewBag.CourseID = courseID; 
            return View(enrolledStudents);
        }
    }
}