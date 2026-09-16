// StudentController.cs
using LearnLink.App_Start;
using System.Linq;
using System.Web.Mvc;
using LearnLink.Models;

namespace LearnLink.Controllers.Students
{
    [CustomAuthorize(Roles = "Student")]
    public class StudentController : Controller
    {
        
        public ActionResult Courses()
        {
          //  var courses = db.Courses.ToList();
            return View();
        }

     

        protected override void Dispose(bool disposing)
        {
          
            base.Dispose(disposing);
        }
    }
}
