using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class ManageCourseController : Controller
    {
        string constr = DBconnection.connStr;
        public ActionResult ManageCourse()
        {
            int teacherID = (int)Session["UserID"];
            List<Course> courses = new List<Course>();

          
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT CourseID, CourseName,CourseCreateDate FROM Courses WHERE TeacherID = @TeacherID";
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
                                CourseName = reader["CourseName"] != DBNull.Value ? reader["CourseName"].ToString() : "No Name",
                                CourseCreateDate = reader["CourseCreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CourseCreateDate"]) : DateTime.MinValue
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(courses); 
        }

        public ActionResult ShowCourseMaterials(int courseid)
        {
            List<CourseMaterials> materials = new List<CourseMaterials>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT FileID, CourseID, Name, ContentType, UploadDate FROM CourseMaterials WHERE CourseID = @CourseID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseid);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            materials.Add(new CourseMaterials
                            {
                                FileID = reader["FileID"] != DBNull.Value ? Convert.ToInt32(reader["FileID"]) : 0,
                                CourseID = reader["CourseID"] != DBNull.Value ? Convert.ToInt32(reader["CourseID"]) : 0,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "No Name",
                                ContentType = reader["ContentType"] != DBNull.Value ? reader["ContentType"].ToString() : "Unknown",
                                UploadDate = reader["UploadDate"] != DBNull.Value ? Convert.ToDateTime(reader["UploadDate"]) : DateTime.MinValue
                            });
                        }
                    }
                    con.Close();
                }
            }

            return View(materials);
        }

        public CourseMaterials getMaterial(int fileId)
        {
            CourseMaterials material = null;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT FileID, CourseID, Name, ContentType, Data FROM CourseMaterials WHERE FileID = @FileID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("FileID", fileId);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            material = new CourseMaterials
                            {
                                FileID = reader["FileID"] != DBNull.Value ? Convert.ToInt32(reader["FileID"]) : 0,
                                CourseID = reader["CourseID"] != DBNull.Value ? Convert.ToInt32(reader["CourseID"]) : 0,
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "No Name",
                                ContentType = reader["ContentType"] != DBNull.Value ? reader["ContentType"].ToString() : "Unknown",
                                Data = reader["Data"] != DBNull.Value ? (byte[])reader["Data"] : null
                            };
                        }
                    }
                    con.Close();
                }
            }

            return material;
        }

        public ActionResult ViewMaterial(int fileId)
        {
            CourseMaterials material = getMaterial(fileId);


            if (material == null)
            {
                return HttpNotFound();
            }

            string contentType = material.ContentType;

            Response.AppendHeader("Content-Disposition", "inline; filename=" + material.Name);
            return File(material.Data, contentType);

           
        }
        public ActionResult DownloadMaterial(int fileId)
        {
            CourseMaterials material = getMaterial(fileId);
            return File(material.Data, material.ContentType, material.Name);
        }



        public ActionResult DeleteMaterial(int fileId, int courseId)
        {
            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = "DELETE FROM CourseMaterials WHERE FileID = @FileID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FileID", fileId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return RedirectToAction("ShowCourseMaterials", new { courseid = courseId });
        }


    }
}