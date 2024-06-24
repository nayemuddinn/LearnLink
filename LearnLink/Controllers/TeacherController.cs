// TeacherController.cs
using System.Linq;
using System.Web.Mvc;
using LearnLink.Models;

namespace LearnLink.Controllers
{
    public class TeacherController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: Teacher/Courses
        public ActionResult Courses()
        {
            int teacherId = /* retrieve teacher's ID from session or authentication */
            var courses = db.Courses.Where(c => c.TeacherID == teacherId).ToList();



























            return View(courses);
        }

        // Other actions for managing courses: Create, Edit, Delete

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}





