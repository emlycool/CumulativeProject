using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CumulativeProject.Models.ViewModels;
using CumulativeProject.Models;
using CumulativeProject.Repositories;
using System.Diagnostics;

namespace CumulativeProject.Controllers
{
    /// <summary>
    /// responsible for handling classes related actions and views.
    /// </summary>
    public class ClassController : Controller
    {
        /// <summary>
        /// Repository for accessing classes data from database
        /// </summary>
        protected ClassRepository ClassRepository;

        public ClassController()
        {
            this.ClassRepository = new ClassRepository();
        }

        /// <summary>
        /// Renders view for teachers list
        /// </summary>
        /// <returns>The view containing the list of teachers.</returns>
        [HttpGet]
        [Route("classes", Name = "Class.List")]
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
        [Route("classes/{id}", Name = "Class.Show")]
        public ActionResult Show(int id)
        {
            TeacherClass teacherClass = this.ClassRepository.Find(id);

            if (teacherClass == null)
            {
                return HttpNotFound();
            }
            ShowClassView viewModel = new ShowClassView()
            {
                TeacherClass = teacherClass
            };
            return View("Show", viewModel);
        }

        [HttpGet]
        [Route("classes/create", Name = "Class.Create")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpGet]
        [Route("classes/edit/{id}", Name = "Class.Edit")]
        public ActionResult Edit(int id)
        {
            TeacherClass teacherClass = this.ClassRepository.Find(id);
            Debug.WriteLine("checkinig");
            Debug.WriteLine(teacherClass);
            if (teacherClass == null)
            {
                return HttpNotFound();
            }

            return View("Edit", teacherClass);
        }
    }
}
