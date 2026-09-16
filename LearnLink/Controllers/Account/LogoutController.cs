using System;
using System.Web.Mvc;

namespace LearnLink.Controllers.Account
{
    public class LogoutController : Controller
    {
        [HttpGet]
        public ActionResult Logout()
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Clear persistent cookies
            CookieHelper.ClearLoginCookies();

            return RedirectToAction("Login", "Login");
        }
    }
}
