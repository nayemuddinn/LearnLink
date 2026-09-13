using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

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
        public ActionResult reg(User user)
        {
            System.Diagnostics.Debug.WriteLine(
                "===== REGISTRATION START ====="
            );

            try
            {
                // -------------------------------------------------
                // BASIC VALIDATION
                // -------------------------------------------------

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid registration data."
                    });
                }

                System.Diagnostics.Debug.WriteLine(
                    "Name: " + user.Name
                );

                System.Diagnostics.Debug.WriteLine(
                    "Email: " + user.Email
                );

                System.Diagnostics.Debug.WriteLine(
                    "Role: " + user.Role
                );

                System.Diagnostics.Debug.WriteLine(
                    "Phone: " + user.Phone
                );

                if (string.IsNullOrWhiteSpace(user.Role))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please select Teacher or Student."
                    });
                }

                if (string.IsNullOrWhiteSpace(user.Password) ||
                    string.IsNullOrWhiteSpace(user.ConfirmPassword))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Password fields cannot be empty."
                    });
                }

                // -------------------------------------------------
                // PASSWORD CHECK
                // -------------------------------------------------

                if (user.Password != user.ConfirmPassword)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Passwords do not match."
                    });
                }

                if (user.Password.Length < 6)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Password must be at least 6 characters long."
                    });
                }

                // -------------------------------------------------
                // PASSWORD STRENGTH
                // -------------------------------------------------

                bool hasDigit =
                    Regex.IsMatch(user.Password, @"\d");

                bool hasSpecialChar =
                    Regex.IsMatch(
                        user.Password,
                        @"[^a-zA-Z0-9]"
                    );

                if (!hasDigit || !hasSpecialChar)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Password must contain at least one digit and one special character."
                    });
                }

                System.Diagnostics.Debug.WriteLine(
                    "===== VALIDATION PASSED ====="
                );

                // -------------------------------------------------
                // VALIDATE ROLE
                // -------------------------------------------------

                string tableName;

                if (user.Role.Equals(
                    "teacher",
                    StringComparison.OrdinalIgnoreCase))
                {
                    tableName = "teacher";
                }
                else if (user.Role.Equals(
                    "student",
                    StringComparison.OrdinalIgnoreCase))
                {
                    tableName = "student";
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid registration role."
                    });
                }

                // -------------------------------------------------
                // DATABASE CONNECTION
                // -------------------------------------------------

                string connStr = DBconnection.connStr;

                using (SqlConnection conn =
                       new SqlConnection(connStr))
                {
                    conn.Open();

                    System.Diagnostics.Debug.WriteLine(
                        "===== DATABASE CONNECTED ====="
                    );

                    System.Diagnostics.Debug.WriteLine(
                        "Database: " + conn.Database
                    );

                    System.Diagnostics.Debug.WriteLine(
                        "Server: " + conn.DataSource
                    );

                    // -------------------------------------------------
                    // CHECK EMAIL
                    // -------------------------------------------------

                    string checkEmailQuery =
                        "SELECT COUNT(*) FROM " +
                        tableName +
                        " WHERE Email = @Email";

                    using (SqlCommand cmd =
                           new SqlCommand(
                               checkEmailQuery,
                               conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Email",
                            user.Email
                        );

                        int emailCount =
                            (int)cmd.ExecuteScalar();

                        System.Diagnostics.Debug.WriteLine(
                            "Email count: " + emailCount
                        );

                        if (emailCount > 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Email already exists!"
                            });
                        }
                    }

                    // -------------------------------------------------
                    // CHECK PHONE
                    // -------------------------------------------------

                    string checkPhoneQuery =
                        "SELECT COUNT(*) FROM " +
                        tableName +
                        " WHERE Phone = @Phone";

                    using (SqlCommand cmd =
                           new SqlCommand(
                               checkPhoneQuery,
                               conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Phone",
                            user.Phone
                        );

                        int phoneCount =
                            (int)cmd.ExecuteScalar();

                        System.Diagnostics.Debug.WriteLine(
                            "Phone count: " + phoneCount
                        );

                        if (phoneCount > 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message =
                                    "Phone number already exists!"
                            });
                        }
                    }

                    // -------------------------------------------------
                    // HASH PASSWORD
                    // -------------------------------------------------

                    System.Diagnostics.Debug.WriteLine(
                        "===== START HASHING ====="
                    );

                    string hashPass =
                        PasswordHasher.HashPassword(
                            user.Password
                        );

                    System.Diagnostics.Debug.WriteLine(
                        "===== HASHING COMPLETE ====="
                    );

                    // -------------------------------------------------
                    // INSERT USER
                    // -------------------------------------------------

                    string query =
                        "INSERT INTO " +
                        tableName +
                        " (Name, Email, Password, Phone, Address, Institution) " +
                        "VALUES (@Name, @Email, @Password, @Phone, @Address, @Institution)";

                    using (SqlCommand cmd =
                           new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Name",
                            user.Name
                        );

                        cmd.Parameters.AddWithValue(
                            "@Email",
                            user.Email
                        );

                        cmd.Parameters.AddWithValue(
                            "@Password",
                            hashPass
                        );

                        cmd.Parameters.AddWithValue(
                            "@Phone",
                            user.Phone
                        );

                        cmd.Parameters.AddWithValue(
                            "@Address",
                            user.Address
                        );

                        cmd.Parameters.AddWithValue(
                            "@Institution",
                            user.Institution
                        );

                        System.Diagnostics.Debug.WriteLine(
                            "===== ABOUT TO INSERT ====="
                        );

                        int rowsAffected =
                            cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine(
                            "===== INSERT RESULT: " +
                            rowsAffected +
                            " ====="
                        );

                        if (rowsAffected > 0)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "===== REGISTRATION SUCCESSFUL ====="
                            );

                            return Json(new
                            {
                                success = true,
                                message =
                                    "Registration successful!"
                            });
                        }

                        return Json(new
                        {
                            success = false,
                            message =
                                "Registration failed. No record was inserted."
                        });
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine(
                    "===== SQL ERROR ====="
                );

                System.Diagnostics.Debug.WriteLine(
                    "Message: " + sqlEx.Message
                );

                System.Diagnostics.Debug.WriteLine(
                    "Number: " + sqlEx.Number
                );

                return Json(new
                {
                    success = false,
                    message =
                        "Database error. Please try again."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "===== GENERAL ERROR ====="
                );

                System.Diagnostics.Debug.WriteLine(
                    "Message: " + ex.Message
                );

                return Json(new
                {
                    success = false,
                    message =
                        "An error occurred. Please try again."
                });
            }
        }
    }
}

