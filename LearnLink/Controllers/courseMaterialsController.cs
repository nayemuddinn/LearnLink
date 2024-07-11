using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web.Mvc;
using System.Data;
using System.Web;

namespace LearnLink.Controllers
{
    public class CourseMaterialsController : Controller
    {
        string connStr = DBconnection.connStr;

        public ActionResult CourseMaterials()
        {

            return View();
        }
        [HttpPost]
        public ActionResult CourseMaterials(int courseID, HttpPostedFileBase fileUpload)
        {

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                try
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    string contentType = fileUpload.ContentType;
                        using (Stream fs = fileUpload.InputStream)
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                byte[] bytes = br.ReadBytes((int)fs.Length);

                                CourseMaterials material = new CourseMaterials
                                {
                                    CourseID = courseID,
                                    Name = filename,
                                    TeacherID = (int)Session["UserID"],
                                    ContentType = contentType,
                                    Data = bytes,
                                    UploadDate = DateTime.Now
                                };

                                string constr = DBconnection.connStr;

                            bool courseExists = false;
                            using (SqlConnection con = new SqlConnection(constr))
                            {
                                string checkQuery = "SELECT COUNT(1) FROM Courses WHERE CourseID = @CourseID";
                                using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@CourseID", courseID);
                                    con.Open();
                                    courseExists = (int)cmd.ExecuteScalar() > 0;
                                    con.Close();
                                }
                            }

                            if (!courseExists)
                            {
                                Response.Write("<script>alert('No Courses Found!');</script>");         
                                return View();
                            }

                            using (SqlConnection con = new SqlConnection(constr))
                                {
                                    string query = "INSERT INTO courseMaterials (CourseID, TeacherID, Name, ContentType, Data, UploadDate) VALUES (@CourseID, @TeacherID, @Name, @ContentType, @Data, @UploadDate)";

                  
                                    using (SqlCommand cmd = new SqlCommand(query, con))
                                    {
      
                                        cmd.Parameters.AddWithValue("@CourseID", material.CourseID);       
                                        cmd.Parameters.AddWithValue("@TeacherID", material.TeacherID);     
                                        cmd.Parameters.AddWithValue("@Name", material.Name);              
                                        cmd.Parameters.AddWithValue("@ContentType", material.ContentType); 
                                        cmd.Parameters.AddWithValue("@Data", material.Data);         
                                        cmd.Parameters.AddWithValue("@UploadDate", material.UploadDate); 
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        con.Close();
                                        Response.Write("<script>alert('File Uploaded Successfully');</script>");
                                }
                                }
                            }
                        }
                      
                 
                }
                catch (SqlException sqlEx)
                {
                    Response.Write("<script>alert('Error uploading file to the database. Please try again');</script>");
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('File Uploading Failed, Try Again !');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Upload a valid File');</script>");
  
            }

            return View();
        }
    }
}