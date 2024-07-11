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

                    if (Path.GetExtension(filename).ToLower() == ".pdf")
                    {
                        using (Stream fs = fileUpload.InputStream)
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                byte[] bytes = br.ReadBytes((int)fs.Length);

                                CourseMaterials material = new CourseMaterials
                                {
                                    CourseID = courseID,
                                    Name = filename,
                                    TeacherID = Session["UserID"],
                                    ContentType = contentType,
                                    Data = bytes,
                                    UploadDate = DateTime.Now
                                };

                                string constr = DBconnection.connStr;

   
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
                                    }
                                }
                            }
                        }
                        Response.Write("<script>alert('File Uploaded Successfully');</script>");
                    }
                    else
                    {  Response.Write("<script>alert('Please Choose a File');</script>");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('"+"AShe na "+ex.Message+"');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Uploadddd Failed');</script>");
  
            }

            return View();
        }
    }
}