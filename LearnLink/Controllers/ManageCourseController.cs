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
            int teacherID = Convert.ToInt32(Session["UserID"]); 
            List<CourseMaterials> courses = new List<CourseMaterials>();

            string constr = DBconnection.connStr;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT CourseID, Name, ContentType, UploadDate FROM courseMaterials WHERE TeacherID = @TeacherID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new CourseMaterials
                            {
                                CourseID = Convert.ToInt32(reader["CourseID"]),
                                Name = reader["Name"].ToString(),
                                ContentType = reader["ContentType"].ToString(),
                                UploadDate = Convert.ToDateTime(reader["UploadDate"])
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
