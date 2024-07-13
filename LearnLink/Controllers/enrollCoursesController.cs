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

        public string getStatus(int courseId)
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

     
        public ActionResult EnrollCourse(int courseId, int teacherId)
        {
            string status = getStatus(courseId);

            if (status == null)
            {
                using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
                {
                    string insertQuery = "INSERT INTO Enrollment (StudentID, CourseID, TeacherID, RequestDate, Status) VALUES (@StudentID, @CourseID, @TeacherID, @RequestDate, @Status)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@StudentID", Session["UserID"]);
                    insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                    insertCmd.Parameters.AddWithValue("@TeacherID", teacherId);
                    insertCmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@Status", "Requested");

                    conn.Open();
                    insertCmd.ExecuteNonQuery();
                    ViewBag.Message = "Course Enrollment Request Sent";
                }
            }
            else if (status == "Requested")
            {
                ViewBag.Message = "You have already sent a request for this course.";
            }
            else if (status == "Accepted")
            {
                ViewBag.Message = "You are already enrolled in this course.";
            }
            else if (status == "Rejected")
            {
                ViewBag.Message = "Rejected";
                ViewBag.CourseID = courseId;
                
            }

            return View();
        }

        public ActionResult ReEnrollCourse(int courseId)
        {
          

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "UPDATE Enrollment SET Status = @Status, RequestDate = @RequestDate WHERE StudentID = @StudentID AND CourseID = @CourseID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", Session["UserID"]);
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                cmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Status", "Requested");

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    ViewBag.Message = "Course Enrollment Request Sent Again";
                }
                else
                {
                    ViewBag.Message = "Failed to update the enrollment request. Please try again.";
                }
            }
            return View();


        }


    }
}
