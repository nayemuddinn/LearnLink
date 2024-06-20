using LearnLink.Models;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            // Dummy check for demonstration
            if (user.Email.Equals("student@gmail.com") && user.Password.Equals("1234"))
            {
                user.Name = "Student Name";
                user.UserRole = "Student";
                Session["User"] = user.Name;
                Session["UserRole"] = user.UserRole;
                Session["UserEmail"] = user.Email;
                return RedirectToAction("Dashboard", "Student");
            }
            else if (user.Email.Equals("teacher@gmail.com") && user.Password.Equals("1234"))
            {
                user.Name = "Teacher Name";
                user.UserRole = "Teacher";
                Session["User"] = user.Name;
                Session["UserRole"] = user.UserRole;
                Session["UserEmail"] = user.Email;
                return RedirectToAction("Dashboard", "Teacher");
            }
            else
            {
                ViewBag.Log = "Login Failed";
            }

            return View();
        }
    }
}
