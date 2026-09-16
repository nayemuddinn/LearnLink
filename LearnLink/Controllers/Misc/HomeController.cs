using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LearnLink.Controllers.Account;

namespace LearnLink.Controllers.Misc
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Check if user is logged in via session
            if (Session["UserID"] != null && Session["UserRole"] != null)
            {
                System.Diagnostics.Debug.WriteLine("✓ User has active session, redirecting to dashboard");
                return RedirectToDashboard();
            }

            // Check if user has valid cookies and restore session
            if (CookieHelper.HasValidLoginCookies())
            {
                System.Diagnostics.Debug.WriteLine("✓ Valid cookies found, attempting to restore session");
                bool restored = CookieHelper.RestoreSessionFromCookies(Session);

                if (restored && Session["UserID"] != null && Session["UserRole"] != null)
                {
                    System.Diagnostics.Debug.WriteLine("✓ Session restored from cookies, redirecting to dashboard");
                    return RedirectToDashboard();
                }
            }

            return View();
        }

        /// <summary>
        /// Helper method to redirect user to their role-specific dashboard
        /// </summary>
        private ActionResult RedirectToDashboard()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString();

                if (string.IsNullOrEmpty(userRole))
                {
                    System.Diagnostics.Debug.WriteLine("✗ UserRole is null/empty");
                    return View();
                }

                // Redirect based on role
                if (userRole.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Redirecting Teacher to TeacherDashboard");
                    return RedirectToAction("Dashboard", "TeacherDashboard");
                }
                else if (userRole.Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Redirecting Student to StudentDashboard");
                    return RedirectToAction("Dashboard", "StudentDashboard");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✗ Unknown role: {userRole}");
                    return View();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error in RedirectToDashboard: {ex.Message}");
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
