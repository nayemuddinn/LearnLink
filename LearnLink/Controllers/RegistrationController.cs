using LearnLink.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace LearnLink.Controllers
{

    public class RegistrationController : Controller
    {


        public ActionResult reg()
        {

            return View();
        }

        [HttpPost]
        public ActionResult reg(User user)
        {
            string connStr = "Data Source = DAREDEVIL\\SQLEXPRESS; Initial Catalog = learnlink; Integrated Security = True; TrustServerCertificate = True";
         
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    // Response.Write("<script>alert('Ashlo!');</script>");
                    conn.Open();
                    string query = "INSERT INTO student (Name, Email, Password, Phone, Address, Institution) VALUES('" + user.Name + "','" + user.Email + "','" + user.Password + "','" + user.Phone + "','" + user.Address + "','" + user.Institution + "')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("Name", user.Name);
                    cmd.Parameters.AddWithValue("Email", user.Email);
                    cmd.Parameters.AddWithValue("Password", user.Password);
                    cmd.Parameters.AddWithValue("Phone", user.Phone);
                    cmd.Parameters.AddWithValue("Address", user.Phone);
                    cmd.Parameters.AddWithValue("Institution", user.Institution);

                    cmd.ExecuteNonQuery();
                    Response.Write("<script>alert('Registration successful!');</script>");
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
                }
                return View();
            }
        }
    }
}