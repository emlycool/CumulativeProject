using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CumulativeProject.Models;
using CumulativeProject.Repositories;

namespace CumulativeProject.Controllers
{
    /// <summary>
    /// responsible for handling teacher data-related API endpoints.
    /// </summary>
    public class TeacherDataController : ApiController
    {
        private readonly TeacherRepository TeacherRepository;

        public TeacherDataController() { 
            this.TeacherRepository = new TeacherRepository();
        }

        /// <summary>
        /// Get list of teachers optionally filtered by a search query.
        /// </summary>
        /// <param name="searchQuery">Optional search query to filter teachers.</param>
        /// <returns>
        /// If no search query is provided, returns all teachers.
        /// If a search query is provided, returns teachers filtered by the query.
        /// </returns>
        [HttpGet]
        [Route("api/teachers")]
        public IHttpActionResult Teachers(string searchQuery = null)
        {
            if (searchQuery == null)
            {
                return Ok(this.TeacherRepository.All());
            }
            else
            {
                return Ok(this.TeacherRepository.Search(searchQuery));
            }
        }

        [HttpPost]
        [Route("api/teachers")]
        public IHttpActionResult StoreTeacher([FromBody]Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                    .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);


                return Json(new { success = false, errors = errors });
            }

            return Ok(teacher);
        }
    }
}
