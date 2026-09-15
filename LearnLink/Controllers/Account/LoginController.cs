using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using LearnLink.Content;

namespace LearnLink.Controllers.Account
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            string connStr = DBconnection.connStr;
            string role = user.Role;

            // Normalize or validate role to match lowercase table names ("teacher" or "student")
            string tableName = null;
            if (role != null && role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                tableName = "teacher";
            }
            else if (role != null && role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                tableName = "student";
            }

            if (tableName == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid login role."
                });
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Added IsVerified to the SELECT columns
                    string query =
                        "SELECT name, password, userID, IsVerified FROM " +
                        tableName +
                        " WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", user.Email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["Password"].ToString();
                                bool isVerified = Convert.ToBoolean(reader["IsVerified"]);

                                // 1. CHECK IF EMAIL IS VERIFIED
                                if (!isVerified)
                                {
                                    return Json(new
                                    {
                                        success = false,
                                        message = "Your email is not verified. Please check your inbox for the verification link."
                                    });
                                }

                                // 2. VERIFY PASSWORD
                                bool passwordCorrect =
                                    PasswordHasher.VerifyPassword(
                                        user.Password,
                                        storedPassword
                                    );

                                if (passwordCorrect)
                                {
                                    Session["UserRole"] = role;
                                    Session["UserName"] = reader["Name"].ToString();
                                    Session["UserEmail"] = user.Email;
                                    Session["UserID"] = reader["UserID"];

                                    string redirectUrl = "";

                                    if (tableName.Equals("teacher", StringComparison.OrdinalIgnoreCase))
                                    {
                                        redirectUrl = Url.Action(
                                            "Dashboard",
                                            "TeacherDashboard"
                                        );
                                    }
                                    else if (tableName.Equals("student", StringComparison.OrdinalIgnoreCase))
                                    {
                                        redirectUrl = Url.Action(
                                            "Dashboard",
                                            "StudentDashboard"
                                        );
                                    }

                                    return Json(new
                                    {
                                        success = true,
                                        redirectUrl = redirectUrl
                                    });
                                }
                            }

                            // Email doesn't exist OR password is wrong
                            return Json(new
                            {
                                success = false,
                                message = "Wrong email or password."
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Something went wrong. Please try again."
                    });
                }
            }
        }
    }
}