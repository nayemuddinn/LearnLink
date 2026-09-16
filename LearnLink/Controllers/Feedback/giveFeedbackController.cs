using LearnLink.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers.Feedback
{
    [CustomAuthorize]
    public class giveFeedbackController : Controller
    {
        // GET: giveFeedback
        public ActionResult giveFeedback()
        {
            return View();
        }
    }
}
