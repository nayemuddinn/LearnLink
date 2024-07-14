using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Helpers;
using LearnLink.Content;


namespace LearnLink.Controllers
{

    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            string connStr = DBconnection.connStr;
            string role = user.Role;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT name,password,userID FROM " + role + " WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", user.Email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && PasswordHasher.VerifyPassword(user.Password, reader["Password"].ToString()))
                            {
                                Session["UserRole"] = role.ToString();
                                Session["UserName"] = reader["Name"].ToString();
                                Session["UserEmail"] = user.Email;
                                Session["UserID"] = reader["UserID"];
                                Response.Write("<script>alert('Login successful!');</script>");

                                if (role.Equals("teacher", StringComparison.OrdinalIgnoreCase))
                                {
                                    return RedirectToAction("Dashboard", "TeacherDashboard");
                                }
                                else if (role.Equals("student", StringComparison.OrdinalIgnoreCase))
                                {
                                    return RedirectToAction("Dashboard", "StudentDashboard");
                                }
                            }
                            else
                            {
                                Response.Write("<script>alert('Wrong credential.');</script>");
                                return View();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Try Again');</script>");
                }
            }

            return View();
        }
    }
}