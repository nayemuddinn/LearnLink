﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class StudentDashboardController : Controller
    {
        public ActionResult Dashboard()
        {
            if (ViewBag.TLE != null)
            {
                Response.Write("<script>alert('" + ViewBag.TLE + "');</script>");
                ViewBag.TLE = null;
            }
            return View();
        }
    }
}