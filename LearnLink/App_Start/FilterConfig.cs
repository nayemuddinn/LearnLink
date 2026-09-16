using System.Web;
using System.Web.Mvc;
using LearnLink.Controllers;

namespace LearnLink
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CookieAuthenticationFilter());
        }
    }
}
