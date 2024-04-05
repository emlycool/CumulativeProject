using System.Web.Mvc;
using CumulativeProject.Models;
using CumulativeProject.Repositories;

namespace CumulativeProject.Controllers
{

    /// <summary>
    /// responsible for handling teachers related actions and views.
    /// </summary>
    public class TeacherController : Controller
    {

        /// <summary>
        /// Repository for accessing teacher data from database
        /// </summary>
        protected TeacherRepository TeacherRepository;
        public TeacherController() { 
            this.TeacherRepository = new TeacherRepository();
        }

        /// <summary>
        /// Renders view for teachers list
        /// </summary>
        /// <returns>The view containing the list of teachers.</returns>
        [HttpGet]
        [Route("teachers", Name = "Teachers.List")]
        public ActionResult Index()
        {
            return View("List");
        }

        /// <summary>
        /// renders view details of a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to display.</param>
        /// <returns>
        /// If the teacher is found, returns the view containing details of the teacher.
        /// If the teacher is not found, returns a HTTP 404 Not Found error.
        /// </returns>
        [HttpGet]
        [Route("teachers/{id}", Name = "Teachers.Show")]
        public ActionResult Show(int id) {
            Teacher teacher = this.TeacherRepository.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View("Show", teacher);
        }

        [HttpGet]
        [Route("teachers/create", Name = "Teachers.Create")]
        public ActionResult Create()
        {
            return View("Create");
        }
    }
}