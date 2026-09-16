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
           
            if (Session["UserID"] != null && Session["UserRole"] != null)
            {
                System.Diagnostics.Debug.WriteLine(" User already has active session, redirecting to dashboard");
                return RedirectToDashboard();
            }

    
            if (CookieHelper.HasValidLoginCookies())
            {
                System.Diagnostics.Debug.WriteLine("✓ Valid cookies found, attempting to restore session");
                bool restored = CookieHelper.RestoreSessionFromCookies(Session);

                if (restored && Session["UserID"] != null && Session["UserRole"] != null)
                {
                    System.Diagnostics.Debug.WriteLine("✓ Session restored from cookies successfully");
                    return RedirectToDashboard();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("✗ Failed to restore session from cookies");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No valid cookies found");
            }

            return View();
        }

      
        private ActionResult RedirectToDashboard()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString();

                if (string.IsNullOrEmpty(userRole))
                {
                    System.Diagnostics.Debug.WriteLine("✗ UserRole is null/empty, redirecting to Login");
                    return RedirectToAction("Login", "Login");
                }

                // Redirect based on role
                if (userRole.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($" Redirecting Teacher to TeacherDashboard");
                    return RedirectToAction("Dashboard", "TeacherDashboard");
                }
                else if (userRole.Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($" Redirecting Student to StudentDashboard");
                    return RedirectToAction("Dashboard", "StudentDashboard");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($" Unknown role: {userRole}");
                    return RedirectToAction("Login", "Login");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Error in RedirectToDashboard: {ex.Message}");
                return RedirectToAction("Login", "Login");
            }
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

                                    // Set persistent cookies for "remember me" functionality
                                    int userId = Convert.ToInt32(reader["UserID"]);
                                    CookieHelper.SetLoginCookies(userId, role, reader["Name"].ToString(), user.Email);

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