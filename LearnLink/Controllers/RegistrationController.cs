using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Mvc;

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
     
            if (!user.Password.Equals(user.ConfirmPassword))
            {
                TempData["AlertMessage"] = "Passwords do not Match!";
                return View();
            }

            if (!user.PIN.Equals(user.ConfirmPIN))
            {
                TempData["AlertMessage"] = "PIN do not Match!";
                return View();
            }

            if (user.Password.Length < 6)
            {
                TempData["AlertMessage"] = "Password must be at least 6 characters long.";
                return View();
            }

            bool hasDigit = Regex.IsMatch(user.Password, @"\d");
            bool hasSpecialChar = Regex.IsMatch(user.Password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]");

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

                    string checkEmailQuery = "SELECT COUNT(*) FROM " + user.Role + " WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(checkEmailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        int emailCount = (int)cmd.ExecuteScalar();
                        if (emailCount > 0)
                        {
                            TempData["AlertMessage"] = "Email already exists!";
                            return View();
                        }
                    }

                    string checkPhoneQuery = "SELECT COUNT(*) FROM " + user.Role + " WHERE Phone = @Phone";
                    using (SqlCommand cmd = new SqlCommand(checkPhoneQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", user.Phone);
                        int phoneCount = (int)cmd.ExecuteScalar();
                        if (phoneCount > 0)
                        {
                            TempData["AlertMessage"] = "Phone number already exists!";
                            return View();
                        }
                    }

                    string Hashpass = PasswordHasher.HashPassword(user.Password);
                    string Hashpin = PasswordHasher.HashPassword(user.PIN);
                    string query = "INSERT INTO " + user.Role + " (Name, Email, Password, Phone, Address, Institution,PIN) " +
                                   "VALUES (@Name, @Email, @Password, @Phone, @Address, @Institution,@PIN)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", Hashpass);
                        cmd.Parameters.AddWithValue("@Phone", user.Phone);
                        cmd.Parameters.AddWithValue("@Address", user.Address);
                        cmd.Parameters.AddWithValue("@Institution", user.Institution);
                        cmd.Parameters.AddWithValue("@PIN",Hashpin);

                        cmd.ExecuteNonQuery();
                    }

                    ViewBag.SuccessMessage = "Registration successful!";
                }
                catch (SqlException sqlEx)
                {
                    ViewBag.ErrorMessage = "SQL Error: " + sqlEx.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error: " + ex.Message;
                }

                return View();
            }
        }

    }
}

