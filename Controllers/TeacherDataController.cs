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
    /// responsible for handling teacher data-related API endpoints.
    /// </summary>
    public class TeacherDataController : ApiController
    {
        private readonly TeacherRepository TeacherRepository;
        private readonly ClassRepository ClassRepository;

        public TeacherDataController() { 
            this.TeacherRepository = new TeacherRepository();
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
        [Route("api/teachers")]
        public IHttpActionResult Teachers(string searchQuery = null)
        {
            if (searchQuery == null)
            {
                //return Ok(this.TeacherRepository.All());
                return ResponseHelper.JsonResponse("Teachers retrieved successfully", HttpStatusCode.OK, true, data: this.TeacherRepository.All());
            }
            else
            {
                //return Ok(this.TeacherRepository.Search(searchQuery));
                return ResponseHelper.JsonResponse("Teachers retrieved successfully", HttpStatusCode.OK, true, data: this.TeacherRepository.Search(searchQuery));
            }
        }


        /// <summary>
        /// Creates a new teacher record in the database.
        /// </summary>
        /// <request>
        /// Route: POST /api/teachers
        /// </request>
        /// <param name="teacher">The teacher object to be stored.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation along with the stored teacher data.
        /// </returns>
        /// <example>
        /// <code>
        /// curl -X POST -k -H "Content-Type: application/json" -d "{\"firstName\": \"John\", \"lastName\": \"Doe\", \"employeeNumber\": \"12345\", \"hireDate\": \"2024-04-05T00:00:00\", \"salary\": 50000}" https://localhost:44302/api/teachers
        /// </code>
        /// </example>
        /// <response code="201">Returns the newly created teacher object with status code 201 Created.</response>
        /// <response code="400">If the request is invalid or teacher data is null.</response>
        [HttpPost]
        [Route("api/teachers")]
        public IHttpActionResult StoreTeacher([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data is null.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

                return ResponseHelper.JsonResponse("Request is invalid", HttpStatusCode.BadRequest, false, errors: errors);
            }

            Teacher storedTeacher = this.TeacherRepository.StoreTeacher(teacher);

            return ResponseHelper.JsonResponse("Teacher created successfully", HttpStatusCode.Created, true, data: storedTeacher);
        }


        /// <summary>
        /// Updates an existing teacher record in the database.
        /// </summary>
        /// <request>
        /// Route: POST /api/teachers/{id}
        /// </request>
        /// <param name="id">The ID of the teacher to be updated.</param>
        /// <param name="teacher">The updated teacher object.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation along with the updated teacher data.
        /// </returns>
        /// <example>
        /// <code>
        /// curl -X POST -k -H "Content-Type: application/json" -d "{\"firstName\": \"John\", \"lastName\": \"Doe\", \"employeeNumber\": \"12345\", \"hireDate\": \"2024-04-05T00:00:00\", \"salary\": 60000}" https://localhost:44302/api/teachers/28
        /// </code>
        /// </example>
        /// <response code="200">Returns the updated teacher object with status code 200 OK.</response>
        /// <response code="400">If the request is invalid or the teacher does not exist.</response>
        [HttpPost, HttpPut]
        [Route("api/teachers/{id}")]
        public IHttpActionResult UpdateTeacher(int id, [FromBody]Teacher teacher)
        {
            Teacher currentTeacherData = this.TeacherRepository.Find(id);
            if (currentTeacherData == null || teacher == null)
            {
                return ResponseHelper.JsonResponse("Teacher does not exists", HttpStatusCode.BadRequest, false);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

                return ResponseHelper.JsonResponse("Request is invalid", HttpStatusCode.BadRequest, false, errors: errors);
            }
            teacher.Id = id;
            Teacher updatedTeacher = this.TeacherRepository.UpdateTeacher(teacher);

            return ResponseHelper.JsonResponse("Teacher updated successfully", HttpStatusCode.OK, true, data: updatedTeacher);
        }

        /// <summary>
        /// Deletes an existing teacher record from the database.
        /// </summary>
        /// <remarks>
        /// Route: DELETE /api/teachers/{id}
        /// </remarks>
        /// <param name="id">The ID of the teacher to be deleted.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        /// <example>
        /// <code>
        /// curl -X DELETE -k https://localhost:44302/api/teachers/1
        /// </code>
        /// </example>
        /// <response code="200">If the teacher is successfully deleted.</response>
        /// <response code="400">If the teacher does not exist.</response>
        [HttpDelete]
        [Route("api/teachers/{id}")]
        public IHttpActionResult DeleteTeacher(int id)
        {
            this.TeacherRepository.DeleteTeacher(id);
            return ResponseHelper.JsonResponse("Teacher deleted successfully", HttpStatusCode.OK, true);
        }
    }
}
