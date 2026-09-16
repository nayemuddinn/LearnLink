using System;
using System.Web.Mvc;

namespace LearnLink.App_Start
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
      
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            if (filterContext.HttpContext.Session["UserID"] == null)
            {
               
                System.Diagnostics.Debug.WriteLine(" No UserID in session, redirecting to Login");
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Login"
                    })
                );
                return;
            }

            // If Roles are specified, check user's role
            if (!string.IsNullOrEmpty(Roles))
            {
                object userRoleObj = filterContext.HttpContext.Session["UserRole"];

                if (userRoleObj == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ No UserRole in session, redirecting to Login");
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(new
                        {
                            controller = "Login",
                            action = "Login"
                        })
                    );
                    return;
                }

                string userRole = userRoleObj.ToString();
                string[] allowedRoles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

             
                bool isAuthorized = false;
                foreach (string role in allowedRoles)
                {
                    if (userRole.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        isAuthorized = true;
                        break;
                    }
                }

                if (!isAuthorized)
                {
                   
                    System.Diagnostics.Debug.WriteLine($" User role '{userRole}' is not authorized for this page. Required roles: {Roles}");


                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "Index"
                        })
                    );
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✓ User with role '{userRole}' is authorized");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("✓ User is authenticated (no specific role required)");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
