using LearnLink.Content;
using LearnLink.Models;
using LearnLink.Services;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LearnLink.Controllers.Account
{
    public class forgotPasswordController : Controller
    {
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string Email, string Role)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Role))
            {
                TempData["AlertMessage"] = "Please provide both email and account type.";
                return View();
            }

            string tableName = Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase) ? "teacher" : "student";
            string connStr = DBconnection.connStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string checkQuery = $"SELECT userID FROM {tableName} WHERE Email = @Email";
                    bool userExists = false;

                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", Email);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            userExists = true;
                        }
                    }

                    if (userExists)
                    {
                        string token = Guid.NewGuid().ToString();
                        DateTime expiry = DateTime.Now.AddHours(1);

                        string updateQuery = $"UPDATE {tableName} SET ResetToken = @Token, ResetTokenExpiry = @Expiry WHERE Email = @Email";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Token", token);
                            cmd.Parameters.AddWithValue("@Expiry", expiry);
                            cmd.Parameters.AddWithValue("@Email", Email);
                            cmd.ExecuteNonQuery();
                        }

                        string resetLink = Url.Action("ResetPassword", "forgotPassword", new { role = tableName, token = token }, Request.Url.Scheme);

                        await EmailService.SendPasswordResetEmailAsync(Email, resetLink);
                    }

                    TempData["AlertMessage"] = "If an account with that email exists, a password reset link has been sent.";
                }
                catch (Exception ex)
                {
                    TempData["AlertMessage"] = "Error: " + ex.Message;
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string role, string token)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(token))
            {
                TempData["AlertMessage"] = "Invalid password reset link.";
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Role = role;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string role, string token, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                TempData["AlertMessage"] = "Passwords do not match!";
                ViewBag.Role = role;
                ViewBag.Token = token;
                return View();
            }

            if (Password.Length < 6)
            {
                TempData["AlertMessage"] = "Password must be at least 6 characters long.";
                ViewBag.Role = role;
                ViewBag.Token = token;
                return View();
            }

            bool hasDigit = Regex.IsMatch(Password, @"\d");
            bool hasSpecialChar = Regex.IsMatch(Password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]");

            if (!hasDigit || !hasSpecialChar)
            {
                TempData["AlertMessage"] = "Password must contain at least one digit and one special character.";
                ViewBag.Role = role;
                ViewBag.Token = token;
                return View();
            }

            string tableName = role.Equals("teacher", StringComparison.OrdinalIgnoreCase) ? "teacher" : "student";
            string connStr = DBconnection.connStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string validateQuery = $"SELECT Email FROM {tableName} WHERE ResetToken = @Token AND ResetTokenExpiry > @Now";
                    string userEmail = null;

                    using (SqlCommand cmd = new SqlCommand(validateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Token", token);
                        cmd.Parameters.AddWithValue("@Now", DateTime.Now);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            userEmail = result.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(userEmail))
                    {
                        TempData["AlertMessage"] = "Invalid or expired password reset link.";
                        return RedirectToAction("ForgotPassword");
                    }

                    string hashedPassword = PasswordHasher.HashPassword(Password);
                    string updateQuery = $"UPDATE {tableName} SET Password = @Password, ResetToken = NULL, ResetTokenExpiry = NULL WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Email", userEmail);
                        cmd.ExecuteNonQuery();
                    }

                    TempData["AlertMessage"] = "Password has been updated successfully. Please log in.";
                    return RedirectToAction("Login", "Login");
                }
                catch (Exception ex)
                {
                    TempData["AlertMessage"] = "Error: " + ex.Message;
                    ViewBag.Role = role;
                    ViewBag.Token = token;
                    return View();
                }
            }
        }
    }
}