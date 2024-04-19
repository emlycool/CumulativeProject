using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CumulativeProject.Helpers;
using CumulativeProject.Models;
using CumulativeProject.Repositories;

namespace CumulativeProject.Controllers
{
    /// <summary>
    /// responsible for handling class data-related API endpoints.
    /// </summary>
    public class ClassDataController : ApiController
    {
        private readonly ClassRepository ClassRepository;

        public ClassDataController()
        {
            this.ClassRepository = new ClassRepository();
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
        [Route("api/classes")]
        public IHttpActionResult Classes(string searchQuery = null)
        {
            if (searchQuery == null)
            {
                return ResponseHelper.JsonResponse("Classes retrieved successfully", HttpStatusCode.OK, true, data: this.ClassRepository.All());
            }
            else
            {
                return ResponseHelper.JsonResponse("Classes retrieved successfully", HttpStatusCode.OK, true, data: this.ClassRepository.Search(searchQuery));
            }
        }

        /// <summary>
        /// Deletes an existing class record from the database.
        /// </summary>
        /// <remarks>
        /// Route: DELETE /api/classes/{id}
        /// </remarks>
        /// <param name="id">The ID of the class to be deleted.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        /// <example>
        /// <code>
        /// curl -X DELETE -k https://localhost:44302/api/classes/1
        /// </code>
        /// </example>
        /// <response code="200">If the class is successfully deleted.</response>
        /// <response code="400">If the class does not exist.</response>
        [HttpDelete]
        [Route("api/classes/{id}")]
        public IHttpActionResult DeleteClass(int id)
        {
            this.ClassRepository.DeleteClass(id);
            return ResponseHelper.JsonResponse("Class deleted successfully", HttpStatusCode.OK, true);
        }

        [HttpPost]
        [Route("api/classes")]
        public IHttpActionResult StoreTeacher([FromBody] TeacherClass teacherClass)
        {
            if (teacherClass == null)
            {
                return BadRequest("Class data is null.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

                return ResponseHelper.JsonResponse("Request is invalid", HttpStatusCode.BadRequest, false, errors: errors);
            }

            TeacherClass storedTeacher = this.ClassRepository.StoreClass(teacherClass);

            return ResponseHelper.JsonResponse("Class created successfully", HttpStatusCode.Created, true, data: storedTeacher);
        }

        [HttpPost, HttpPut]
        [Route("api/classes/{id}")]
        public IHttpActionResult UpdateTeacher(int id, [FromBody] TeacherClass teacherClass)
        {
            TeacherClass currentTeacherData = this.ClassRepository.Find(id);
            if (currentTeacherData == null || teacherClass == null)
            {
                return ResponseHelper.JsonResponse("Class does not exists", HttpStatusCode.BadRequest, false);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

                return ResponseHelper.JsonResponse("Request is invalid", HttpStatusCode.BadRequest, false, errors: errors);
            }
            teacherClass.Id = id;
            TeacherClass updatedTeacher = this.ClassRepository.UpdateClass(teacherClass);

            return ResponseHelper.JsonResponse("Teacher updated successfully", HttpStatusCode.OK, true, data: updatedTeacher);
        }
    }

}
