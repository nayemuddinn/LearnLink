using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using GoogleAuthentication.Services;
using Newtonsoft.Json;
using LearnLink.Models;
using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class signingoogleController : Controller
    {
        // GET: signingoogle
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> signingoogle(string code)
        {
           
            
                var clientID = "596117591616-ufi32thv442eg1a26chrbgas74q1o56i.apps.googleusercontent.com";
                var url = "https://localhost:44397/signingoogle/signingoogle";
                var clientsecret = "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";
                var token = await GoogleAuth.GetAuthAccessToken(code, clientID, clientsecret, url);
                var userprofile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
                var googleUser = JsonConvert.DeserializeObject<GoogleProfile>(userprofile);
                return View();
            
           // catch (Exception ex) { }

            return RedirectToAction("");
        }
        string connstr = DBconnection.connStr;

        public ActionResult reg(GoogleProfile user)
        {
           /* if (user.Password != user.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match!";
                return View();
            }*/

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