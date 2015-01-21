using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.Models;
using ContosoUniversity.DAL;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private IStudentRepository _studentRepository;

        public StudentController()
        {
            this._studentRepository = new StudentRepository(new SchoolContext());
        }

        public StudentController(IStudentRepository studentRepository)
        {
            this._studentRepository = studentRepository;
        }

        //
        // GET: /Student/

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null) {
                page = 1;
            }
            else {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in _studentRepository.GetStudents()
                           select s;
            if (!String.IsNullOrEmpty(searchString)) {
                students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                       || s.FirstMidName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder) {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:  // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Student/Details/5

        public ActionResult Details(int id = 0)
        {
            Student student = _studentRepository.GetStudentByID(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
                [Bind(Include = "LastName, FirstMidName, EnrollmentDate")]
                Student student)
        {
            try {
                if (ModelState.IsValid) {
                    _studentRepository.InsertStudent(student);
                    _studentRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */) {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Student student = _studentRepository.GetStudentByID(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonID, LastName, FirstMidName, EnrollmentDate")] Student student)
        {
            try {
                if (ModelState.IsValid) {
                    _studentRepository.UpdateStudent(student);
                    _studentRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */) {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        //
        // GET: /Student/Delete/5

        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault()) {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = _studentRepository.GetStudentByID(id);
            if (student == null) {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try {
                Student student = _studentRepository.GetStudentByID(id);
                _studentRepository.DeleteStudent(id);
                _studentRepository.Save();
            }
            catch (DataException/* dex */) {
                // uncomment dex and log error. 
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _studentRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}