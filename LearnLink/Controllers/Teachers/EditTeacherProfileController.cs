using LearnLink.App_Start;
using LearnLink.Content;
using LearnLink.Models;
using LearnLink.Controllers.Account;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LearnLink.Controllers.Teachers
{
    [CustomAuthorize(Roles = "Teacher")]
    public class EditTeacherProfileController : Controller
    {
        // GET: editteacherProfile
        public ActionResult editteacherProfile()
        {
            int userId = (int)Session["UserID"];
            User user = GetTeacherById(userId);
            User model = new User
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Institution = user.Institution,
                Password = user.Password
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult editteacherProfile(User model, string CurrentPassword = "", string NewPassword = "", string ConfirmNewPassword = "")
        {
            int userId = (int)Session["UserID"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if user wants to change password
                    if (!string.IsNullOrEmpty(CurrentPassword) || !string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmNewPassword))
                    {
                        // Validate password fields
                        if (string.IsNullOrEmpty(CurrentPassword))
                        {
                            ViewBag.ErrorMessage = "Please enter your current password to change it.";
                            ReloadViewData(userId, model);
                            return View(model);
                        }

                        if (string.IsNullOrEmpty(NewPassword))
                        {
                            ViewBag.ErrorMessage = "Please enter a new password.";
                            ReloadViewData(userId, model);
                            return View(model);
                        }

                        if (string.IsNullOrEmpty(ConfirmNewPassword))
                        {
                            ViewBag.ErrorMessage = "Please confirm your new password.";
                            ReloadViewData(userId, model);
                            return View(model);
                        }

                        if (NewPassword != ConfirmNewPassword)
                        {
                            ViewBag.ErrorMessage = "New password and confirm password do not match.";
                            ReloadViewData(userId, model);
                            return View(model);
                        }

                        if (NewPassword.Length < 6)
                        {
                            ViewBag.ErrorMessage = "New password must be at least 6 characters long.";
                            ReloadViewData(userId, model);
                            return View(model);
                        }

                        // Verify current password
                        User currentUser = GetTeacherById(userId);
                        if (!PasswordHasher.VerifyPassword(CurrentPassword, currentUser.Password))
                        {
                            ViewBag.ErrorMessage = "Current password is incorrect.";
                            ReloadViewData(userId, model);
                            return View(model);
                        }

                        // Update password
                        UpdateTeacherProfile(userId, model, NewPassword, true);
                    }
                    else
                    {
                        // Get current user to check for phone changes
                        User currentUser = GetTeacherById(userId);

                        // Check if phone has changed
                        if (!string.IsNullOrEmpty(model.Phone) && model.Phone != currentUser.Phone)
                        {
                            // Verify phone doesn't exist for another user
                            if (PhoneExistsForAnotherUser(model.Phone, userId))
                            {
                                ViewBag.ErrorMessage = "Phone number already exists. Please use a different phone number.";
                                ReloadViewData(userId, model);
                                return View(model);
                            }
                        }

                        // Update profile without password change
                        UpdateTeacherProfile(userId, model, null, false);
                    }

                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("Dashboard", "TeacherDashboard");
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Message.Contains("Violation of UNIQUE KEY constraint"))
                    {
                        ViewBag.ErrorMessage = "Phone number already exists. Please use a different phone number.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "A database error occurred: " + sqlEx.Message;
                    }
                    ReloadViewData(userId, model);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while updating your profile. Please try again.";
                    ReloadViewData(userId, model);
                    return View(model);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                ReloadViewData(userId, model);
            }
            return View(model);
        }

        private void ReloadViewData(int userId, User model)
        {
            User currentUser = GetTeacherById(userId);
            model.Email = currentUser.Email;
            model.Password = currentUser.Password;
        }

        private User GetTeacherById(int userId)
        {

            User user = null;

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "SELECT Name, Email, Phone, Address, Institution, Password FROM Teacher WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Address = reader["Address"].ToString(),
                            Institution = reader["Institution"].ToString(),
                            Password = reader["Password"].ToString()
                        };
                    }
                }
            }
            return user;
        }

        private void UpdateTeacherProfile(int userId, User model, string newPassword = null, bool updatePassword = false)
        {

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query;
                SqlCommand cmd;

                if (updatePassword && !string.IsNullOrEmpty(newPassword))
                {
                    query = "UPDATE Teacher SET Name = @Name, Phone = @Phone, Address = @Address, Institution = @Institution, Password = @Password WHERE UserID = @UserID";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Phone", model.Phone);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@Institution", model.Institution);
                    cmd.Parameters.AddWithValue("@Password", PasswordHasher.HashPassword(newPassword));
                    cmd.Parameters.AddWithValue("@UserID", userId);
                }
                else
                {
                    query = "UPDATE Teacher SET Name = @Name, Phone = @Phone, Address = @Address, Institution = @Institution WHERE UserID = @UserID";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Phone", model.Phone);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@Institution", model.Institution);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool PhoneExistsForAnotherUser(string phone, int userId)
        {
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "SELECT COUNT(*) FROM Teacher WHERE Phone = @Phone AND UserID != @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
