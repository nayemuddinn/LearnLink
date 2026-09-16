using LearnLink.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers.Misc
{
    [CustomAuthorize]
    public class AssessmentController : Controller
    {

        public ActionResult Assessment()
        {
            return View();
        }
    }
}
