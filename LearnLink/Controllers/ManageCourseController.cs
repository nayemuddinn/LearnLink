using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class ManageCourseController : Controller
    {

        public ActionResult ManageCourse()
        {
            int teacherID = (int)Session["UserID"];
            List<Course> courses = new List<Course>();

            string constr = DBconnection.connStr;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT CourseID, CourseName FROM Courses WHERE TeacherID = @TeacherID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                CourseID = reader["CourseID"] != DBNull.Value ? Convert.ToInt32(reader["CourseID"]) : 0,
                                CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "No Name"
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(courses); 
        }


    }
}
  