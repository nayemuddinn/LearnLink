using System;
using System.Web.Mvc;
using LearnLink.Controllers.Account;

namespace LearnLink.Controllers
{
  
    public class CookieAuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var session = filterContext.HttpContext.Session;

      
                if (session["UserID"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Session UserID is null, checking cookies...");

                    if (CookieHelper.HasValidLoginCookies())
                    {
                        bool restored = CookieHelper.RestoreSessionFromCookies(session);
                        if (restored && session["UserID"] != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"✓ Session restored from cookies. UserID: {session["UserID"]}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("✗ Failed to restore session from cookies");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No valid login cookies found");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Session already active. UserID: {session["UserID"]}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CookieAuthenticationFilter: {ex.Message}");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
