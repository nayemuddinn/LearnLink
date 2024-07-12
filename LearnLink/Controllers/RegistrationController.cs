using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
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
            if (user.Password != user.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match!";
                return View();
            }

            string connStr = DBconnection.connStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string role = user.Role;
                    string Hashpass = PasswordHasher.HashPassword(user.Password);
                    conn.Open();
                    string query = "INSERT INTO " + role + " (Name, Email, Password, Phone, Address, Institution) " +
                                   "VALUES (@Name, @Email, @Password, @Phone, @Address, @Institution)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", Hashpass);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Institution", user.Institution);

                    cmd.ExecuteNonQuery();
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
