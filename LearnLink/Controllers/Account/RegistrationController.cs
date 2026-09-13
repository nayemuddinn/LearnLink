using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace LearnLink.Controllers.Account
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
            System.Diagnostics.Debug.WriteLine("===== REGISTRATION START =====");

            try
            {
                // Check received values
                System.Diagnostics.Debug.WriteLine("Name: " + user.Name);
                System.Diagnostics.Debug.WriteLine("Email: " + user.Email);
                System.Diagnostics.Debug.WriteLine("Role: " + user.Role);
                System.Diagnostics.Debug.WriteLine("Phone: " + user.Phone);

                // -------------------------------------------------
                // BASIC VALIDATION
                // -------------------------------------------------

                if (user == null)
                {
                    TempData["AlertMessage"] = "Invalid registration data.";
                    return View();
                }

                if (string.IsNullOrWhiteSpace(user.Role))
                {
                    TempData["AlertMessage"] = "Please select Teacher or Student.";
                    return View(user);
                }

                if (string.IsNullOrWhiteSpace(user.Password) ||
                    string.IsNullOrWhiteSpace(user.ConfirmPassword))
                {
                    TempData["AlertMessage"] = "Password fields cannot be empty.";
                    return View(user);
                }

                if (string.IsNullOrWhiteSpace(user.PIN) ||
                    string.IsNullOrWhiteSpace(user.ConfirmPIN))
                {
                    TempData["AlertMessage"] = "PIN fields cannot be empty.";
                    return View(user);
                }

                // -------------------------------------------------
                // PASSWORD CHECK
                // -------------------------------------------------

                if (user.Password != user.ConfirmPassword)
                {
                    System.Diagnostics.Debug.WriteLine("STOPPED: Password mismatch");

                    TempData["AlertMessage"] = "Passwords do not match.";
                    return View(user);
                }

                if (user.Password.Length < 6)
                {
                    System.Diagnostics.Debug.WriteLine("STOPPED: Password too short");

                    TempData["AlertMessage"] =
                        "Password must be at least 6 characters long.";

                    return View(user);
                }

                // -------------------------------------------------
                // PIN CHECK
                // -------------------------------------------------

                if (user.PIN != user.ConfirmPIN)
                {
                    System.Diagnostics.Debug.WriteLine("STOPPED: PIN mismatch");

                    TempData["AlertMessage"] = "PINs do not match.";
                    return View(user);
                }

                if (user.PIN.Length < 6)
                {
                    System.Diagnostics.Debug.WriteLine("STOPPED: PIN too short");

                    TempData["AlertMessage"] =
                        "PIN must be at least 6 characters long.";

                    return View(user);
                }

                // -------------------------------------------------
                // PASSWORD STRENGTH
                // -------------------------------------------------

                bool hasDigit = Regex.IsMatch(user.Password, @"\d");

                bool hasSpecialChar = Regex.IsMatch(
                    user.Password,
                    @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]"
                );

                if (!hasDigit || !hasSpecialChar)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "STOPPED: Password does not meet requirements"
                    );

                    TempData["AlertMessage"] =
                        "Password must contain at least one digit and one special character.";

                    return View(user);
                }

                System.Diagnostics.Debug.WriteLine("===== VALIDATION PASSED =====");

                // -------------------------------------------------
                // VALIDATE ROLE
                // -------------------------------------------------

                string tableName;

                if (user.Role.Equals("teacher", StringComparison.OrdinalIgnoreCase))
                {
                    tableName = "teacher";
                }
                else if (user.Role.Equals("student", StringComparison.OrdinalIgnoreCase))
                {
                    tableName = "student";
                }
                else
                {
                    TempData["AlertMessage"] = "Invalid registration role.";
                    return View(user);
                }

                System.Diagnostics.Debug.WriteLine("Table: " + tableName);

                // -------------------------------------------------
                // DATABASE CONNECTION
                // -------------------------------------------------

                string connStr = DBconnection.connStr;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    System.Diagnostics.Debug.WriteLine(
                        "===== DATABASE CONNECTED ====="
                    );

                    // -------------------------------------------------
                    // CHECK EMAIL
                    // -------------------------------------------------

                    string checkEmailQuery =
                        "SELECT COUNT(*) FROM " + tableName +
                        " WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(checkEmailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", user.Email);

                        int emailCount = (int)cmd.ExecuteScalar();

                        System.Diagnostics.Debug.WriteLine(
                            "Email count: " + emailCount
                        );

                        if (emailCount > 0)
                        {
                            TempData["AlertMessage"] = "Email already exists!";
                            return View(user);
                        }
                    }

                    // -------------------------------------------------
                    // CHECK PHONE
                    // -------------------------------------------------

                    string checkPhoneQuery =
                        "SELECT COUNT(*) FROM " + tableName +
                        " WHERE Phone = @Phone";

                    using (SqlCommand cmd = new SqlCommand(checkPhoneQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", user.Phone);

                        int phoneCount = (int)cmd.ExecuteScalar();

                        System.Diagnostics.Debug.WriteLine(
                            "Phone count: " + phoneCount
                        );

                        if (phoneCount > 0)
                        {
                            TempData["AlertMessage"] = "Phone number already exists!";
                            return View(user);
                        }
                    }

                    // -------------------------------------------------
                    // HASH PASSWORD AND PIN
                    // -------------------------------------------------

                    string hashPass = PasswordHasher.HashPassword(user.Password);
                    string hashPin = PasswordHasher.HashPassword(user.PIN);

                    System.Diagnostics.Debug.WriteLine("Password and PIN hashed.");

                    // -------------------------------------------------
                    // INSERT USER
                    // -------------------------------------------------

                    string query =
                        "INSERT INTO " + tableName +
                        " (Name, Email, Password, Phone, Address, Institution, PIN) " +
                        "VALUES (@Name, @Email, @Password, @Phone, @Address, @Institution, @PIN)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", hashPass);
                        cmd.Parameters.AddWithValue("@Phone", user.Phone);
                        cmd.Parameters.AddWithValue("@Address", user.Address);
                        cmd.Parameters.AddWithValue("@Institution", user.Institution);
                        cmd.Parameters.AddWithValue("@PIN", hashPin);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine(
                            "Rows inserted: " + rowsAffected
                        );

                        if (rowsAffected > 0)
                        {
                            TempData["AlertMessage"] =
                                "Registration successful!";

                            System.Diagnostics.Debug.WriteLine(
                                "===== REGISTRATION SUCCESSFUL ====="
                            );
                        }
                        else
                        {
                            TempData["AlertMessage"] =
                                "Registration failed. No record was inserted.";

                            System.Diagnostics.Debug.WriteLine(
                                "===== INSERT FAILED ====="
                            );
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine(
                    "===== SQL ERROR ====="
                );

                System.Diagnostics.Debug.WriteLine(
                    sqlEx.Message
                );

                TempData["AlertMessage"] =
                    "Database error: " + sqlEx.Message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "===== GENERAL ERROR ====="
                );

                System.Diagnostics.Debug.WriteLine(
                    ex.Message
                );

                TempData["AlertMessage"] =
                    "An error occurred: " + ex.Message;
            }

            return View(user);
        }

    }
}
