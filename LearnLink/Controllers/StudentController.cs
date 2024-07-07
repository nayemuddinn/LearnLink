// StudentController.cs
using System.Linq;
using System.Web.Mvc;
using LearnLink.Models;

namespace LearnLink.Controllers
{
    public class StudentController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Student/Courses
        public ActionResult Courses()
        {
          //  var courses = db.Courses.ToList();
            return View();
        }

        // Other actions for students can be added here

        protected override void Dispose(bool disposing)
        {
           /* if (disposing)
            {
                db.Dispose();
            }*/
            base.Dispose(disposing);
        }
    }
}
