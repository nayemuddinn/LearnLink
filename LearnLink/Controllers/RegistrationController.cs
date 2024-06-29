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
            string connStr = "Data Source=DESKTOP-E4E4R1B\\SQLEXPRESS02;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
         
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string role = user.Role;
                    string Hashpass = PasswordHasher.HashPassword(user.Password);
                    conn.Open();
                    string query = "INSERT INTO "+role+" (Name, Email, Password, Phone, Address, Institution) " +
                                   "VALUES (@Name, @Email, @Password, @Phone, @Address, @Institution)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", Hashpass);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Institution", user.Institution);

                    cmd.ExecuteNonQuery();
                    Response.Write("<script>alert('Registration successful!');</script>");
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('"+ex.Message+"');</script>");
                }
                return View();
            }
        }
    }
}