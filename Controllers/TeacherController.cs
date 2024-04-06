using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using CumulativeProject.Models;
using CumulativeProject.Models.ViewModels;
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

        /// <summary>
        /// Repository for accessing classes data from database
        /// </summary>
        protected ClassRepository ClassRepository;

        public TeacherController() { 
            this.TeacherRepository = new TeacherRepository();
            this.ClassRepository = new ClassRepository();
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
            List<TeacherClass> classes = this.ClassRepository.GetTeacherClasses(id);

            ShowTeacherVeiw viewModel = new ShowTeacherVeiw
            {
                Teacher = teacher,
                TeacherClasses = classes
            };

            return View("Show", viewModel);
        }

        [HttpGet]
        [Route("teachers/create", Name = "Teachers.Create")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpGet]
        [Route("teachers/edit/{id}", Name = "Teachers.Edit")]
        public ActionResult Edit(int id)
        {
            Teacher teacher = this.TeacherRepository.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }

            return View("Edit", teacher);
        }
    }
}