using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class forgotPasswordController : Controller
    {
     
            public ActionResult ForgotPassword()
            {
                return View();
            }

            [HttpPost]
            public ActionResult ForgotPassword(string Email, string PIN, string Password, string ConfirmPassword, string Role)
            {
                if (Password != ConfirmPassword)
                {
                    TempData["AlertMessage"] = "Passwords do not match!";
                    return View();
                }

                if (Password.Length < 6)
                {
                    TempData["AlertMessage"] = "Password must be at least 6 characters long.";
                    return View();
                }

                bool hasDigit = Regex.IsMatch(Password, @"\d");
                bool hasSpecialChar = Regex.IsMatch(Password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]");

                if (!hasDigit || !hasSpecialChar)
                {
                    TempData["AlertMessage"] = "Password must contain at least one digit and one special character.";
                    return View();
                }

                string connStr = DBconnection.connStr;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();

                        string tableName = Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase) ? "teacher" : "student";
                        string checkPinQuery = $"SELECT PIN FROM {tableName} WHERE Email = @Email";
                        string storedHashedPin = null;

                        using (SqlCommand cmd = new SqlCommand(checkPinQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", Email);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    storedHashedPin = reader["PIN"].ToString();
                                }
                                else
                                {
                                    TempData["AlertMessage"] = "Invalid email.";
                                    return View();
                                }
                            }
                        }

                        if (!PasswordHasher.VerifyPassword(PIN, storedHashedPin))
                        {
                            TempData["AlertMessage"] = "Invalid PIN.";
                            return View();
                        }

                        string hashedPassword = PasswordHasher.HashPassword(Password);
                        string updatePasswordQuery = $"UPDATE {tableName} SET Password = @Password WHERE Email = @Email";
                        using (SqlCommand cmd = new SqlCommand(updatePasswordQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Password", hashedPassword);
                            cmd.Parameters.AddWithValue("@Email", Email);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                TempData["AlertMessage"] = "Password has been updated successfully.";
                            }
                            else
                            {
                                TempData["AlertMessage"] = "An error occurred while updating the password.";
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        TempData["AlertMessage"] = "SQL Error: " + sqlEx.Message;
                    }
                    catch (Exception ex)
                    {
                        TempData["AlertMessage"] = "Error: " + ex.Message;
                    }
                }

                return View();
            }
        }
    }
