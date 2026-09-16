using System;
using System.Web.Mvc;

namespace LearnLink.App_Start
{
    /// <summary>
    /// Custom authorization attribute that checks if user is logged in
    /// Redirects to Login page if user is not authenticated
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if UserID exists in session (means user is logged in)
            if (filterContext.HttpContext.Session["UserID"] == null)
            {
                // User is not logged in, redirect to login page
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Login"
                    })
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}