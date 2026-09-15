using LearnLink.Content;
using LearnLink.Models;
using LearnLink.Services; // Namespace for your EmailService
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LearnLink.Controllers.Account
{
    public class RegistrationController : Controller
    {
        [HttpGet]
        public ActionResult reg()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> reg(User user) // Marked async to support await EmailService
        {
            try
            {
                // Basic validations & password strength checks remain the same...
                if (user == null)
                    return Json(new { success = false, message = "Invalid registration data." });

                if (string.IsNullOrWhiteSpace(user.Role))
                    return Json(new { success = false, message = "Please select Teacher or Student." });

                if (string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.ConfirmPassword))
                    return Json(new { success = false, message = "Password fields cannot be empty." });

                if (user.Password != user.ConfirmPassword)
                    return Json(new { success = false, message = "Passwords do not match." });

                if (user.Password.Length < 6)
                    return Json(new { success = false, message = "Password must be at least 6 characters long." });

                bool hasDigit = Regex.IsMatch(user.Password, @"\d");
                bool hasSpecialChar = Regex.IsMatch(user.Password, @"[^a-zA-Z0-9]");

                if (!hasDigit || !hasSpecialChar)
                    return Json(new { success = false, message = "Password must contain at least one digit and one special character." });

                string tableName = user.Role.Equals("teacher", StringComparison.OrdinalIgnoreCase) ? "teacher" :
                                   user.Role.Equals("student", StringComparison.OrdinalIgnoreCase) ? "student" : null;

                if (tableName == null)
                    return Json(new { success = false, message = "Invalid registration role." });

                string connStr = DBconnection.connStr;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    await conn.OpenAsync();

                    // 1. CHECK EMAIL & VERIFICATION STATUS
                    int existingUserId = 0;
                    bool isVerified = false;

                    string checkEmailQuery = "SELECT UserID, IsVerified FROM " + tableName + " WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(checkEmailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                existingUserId = reader.GetInt32(0);
                                isVerified = reader.GetBoolean(1);
                            }
                        }
                    }

                    if (existingUserId > 0)
                    {
                        if (isVerified)
                        {
                            return Json(new { success = false, message = "Email already exists and is verified! Please log in." });
                        }
                        else
                        {
                            // Unverified: Generate new token, update DB, and resend verification email
                            string newToken = Guid.NewGuid().ToString();
                            string updateTokenQuery = "UPDATE " + tableName + " SET VerificationToken = @Token WHERE UserID = @UserID";
                            using (SqlCommand updateCmd = new SqlCommand(updateTokenQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@Token", newToken);
                                updateCmd.Parameters.AddWithValue("@UserID", existingUserId);
                                await updateCmd.ExecuteNonQueryAsync();
                            }

                            try
                            {
                                string verificationLink = Url.Action("VerifyEmail", "Registration", new { role = tableName, token = newToken }, protocol: Request.Url.Scheme);
                                await EmailService.SendVerificationEmailAsync(user.Email, verificationLink);

                                return Json(new { success = false, message = "An account with this email already exists but is unverified. A new verification mail has been sent. Please check your mail." });
                            }
                            catch (Exception emailEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Resend email failed for {user.Email}: {emailEx.Message}");
                                return Json(new { success = false, message = $"Account exists but is unverified. Could not send verification email. Please contact support. Error: {emailEx.Message}" });
                            }
                        }
                    }

                    // 2. CHECK PHONE DUPLICATE
                    string checkPhoneQuery = "SELECT COUNT(*) FROM " + tableName + " WHERE Phone = @Phone";
                    using (SqlCommand cmd = new SqlCommand(checkPhoneQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", user.Phone);
                        int phoneCount = (int)await cmd.ExecuteScalarAsync();
                        if (phoneCount > 0)
                        {
                            return Json(new { success = false, message = "Phone number already exists!" });
                        }
                    }

                    // 3. HASH PASSWORD & INSERT NEW USER WITH TOKEN
                    string hashPass = PasswordHasher.HashPassword(user.Password);
                    string verificationToken = Guid.NewGuid().ToString();

                    string insertQuery = "INSERT INTO " + tableName + " (Name, Email, Password, Phone, Address, Institution, IsVerified, VerificationToken) " +
                                         "VALUES (@Name, @Email, @Password, @Phone, @Address, @Institution, 0, @VerificationToken)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", hashPass);
                        cmd.Parameters.AddWithValue("@Phone", user.Phone);
                        cmd.Parameters.AddWithValue("@Address", user.Address);
                        cmd.Parameters.AddWithValue("@Institution", user.Institution);
                        cmd.Parameters.AddWithValue("@VerificationToken", verificationToken);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            // Send initial verification email
                            try
                            {
                                string verificationLink = Url.Action("VerifyEmail", "Registration", new { role = tableName, token = verificationToken }, protocol: Request.Url.Scheme);
                                await EmailService.SendVerificationEmailAsync(user.Email, verificationLink);

                                return Json(new { success = true, message = "Registration successful! Please check your email to verify your account before logging in." });
                            }
                            catch (Exception emailEx)
                            {
                                // Email failed but user is registered - log this and notify user
                                System.Diagnostics.Debug.WriteLine($"Email verification failed for {user.Email}: {emailEx.Message}");
                                return Json(new { success = false, message = $"Registration successful but email could not be sent. Please try again later or contact support. Error: {emailEx.Message}" });
                            }
                        }

                        return Json(new { success = false, message = "Registration failed. No record was inserted." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // 4. ADD VERIFY ACTION TO HANDLE INCOMING EMAIL CLICKS
        [HttpGet]
        public async Task<ActionResult> VerifyEmail(string role, string token)
        {
            if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(token))
            {
                ViewBag.Message = "Invalid verification request.";
                return View("VerificationResult");
            }

            string tableName = role.Equals("teacher", StringComparison.OrdinalIgnoreCase) ? "teacher" :
                               role.Equals("student", StringComparison.OrdinalIgnoreCase) ? "student" : null;

            if (tableName == null)
            {
                ViewBag.Message = "Invalid user role.";
                return View("VerificationResult");
            }

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                await conn.OpenAsync();
                string query = "UPDATE " + tableName + " SET IsVerified = 1, VerificationToken = NULL WHERE VerificationToken = @Token";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        ViewBag.Message = "Email verified successfully! You can now log in.";
                    }
                    else
                    {
                        ViewBag.Message = "Invalid or expired verification link.";
                    }
                }
            }

            return View("VerificationResult");
        }
    }
}