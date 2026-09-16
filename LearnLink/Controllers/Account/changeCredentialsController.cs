using LearnLink.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers.Account
{
    [CustomAuthorize]
    public class changeCredentialsController : Controller
    {
        // GET: changeCredentials
        public ActionResult changeCredentials()
        {
            return View();
        }
    }
}
