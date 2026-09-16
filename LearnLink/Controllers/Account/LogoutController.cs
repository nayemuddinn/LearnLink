using System;
using System.Web.Mvc;

namespace LearnLink.Controllers.Account
{
    public class LogoutController : Controller
    {
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LOGOUT INITIATED ===");
                System.Diagnostics.Debug.WriteLine($"Current UserID before logout: {Session["UserID"]}");

                // Clear session
                Session.Clear();
                Session.Abandon();
                System.Diagnostics.Debug.WriteLine("✓ Session cleared and abandoned");

                // Clear persistent cookies
                CookieHelper.ClearLoginCookies();
                System.Diagnostics.Debug.WriteLine("✓ Login cookies cleared");
                System.Diagnostics.Debug.WriteLine("=== LOGOUT COMPLETED ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error during logout: {ex.Message}\n{ex.StackTrace}");
            }

            return RedirectToAction("Login", "Login");
        }
    }
}
