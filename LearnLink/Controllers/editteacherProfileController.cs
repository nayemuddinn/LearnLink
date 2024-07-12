using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class EditTeacherProfileController : Controller
    {
        // GET: editteacherProfile
        public ActionResult editteacherProfile()
        {
            int userId = (int)Session["UserID"];
            User user = GetTeacherById(userId);
            EditProfileViewModel model = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Institution = user.Institution
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult editteacherProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserID"];
                UpdateTeacherProfile(userId, model);
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Dashboard", "TeacherDashboard");
            }
            return View(model);
        }

        private User GetTeacherById(int userId)
        {
           // string connStr = DBConnection.ConnStr;
            User user = null;

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "SELECT Name, Email, Phone, Address, Institution FROM Teacher WHERE UserID = @UserID";
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
                            Institution = reader["Institution"].ToString()
                        };
                    }
                }
            }
            return user;
        }

        private void UpdateTeacherProfile(int userId, EditProfileViewModel model)
        {
           // string connStr = DBConnection.ConnStr;

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "UPDATE Teacher SET Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, Institution = @Institution WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@Address", model.Address);
                cmd.Parameters.AddWithValue("@Institution", model.Institution);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
