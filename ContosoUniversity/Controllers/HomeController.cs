using System.Linq;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;
using System.Web.Mvc;

namespace ContosoUniversity.Controllers
{

    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext();

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Contoso University";

            return View();
        }

        public ActionResult About()
        {
            var data = from student in db.Students
                       group student by student.EnrollmentDate into dateGroup
                       select new EnrollmentDateGroup() {
                           EnrollmentDate = dateGroup.Key,
                           StudentCount = dateGroup.Count()
                       };
            return View(data);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
