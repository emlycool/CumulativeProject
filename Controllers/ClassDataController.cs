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

        /// <summary>
        /// Creates a new teacher class.
        /// </summary>
        /// <remarks>
        /// <para>This endpoint allows you to create a new teacher class by providing the necessary details in the request body.</para>
        /// <para><b>Example:</b></para>
        /// <code>
        /// curl -X POST -k -H "Content-Type: application/json" -d '{"ClassCode": "ABC123", "StartDate": "2024-04-05", "FinishDate": "2024-05-05", "ClassName": "Mathematics", "TeacherId": 1}' https://localhost:44302/api/classes
        /// </code>
        /// <para><b>Success Response:</b></para>
        /// <code>
        /// {
        ///   "message": "Class created successfully",
        ///   "statusCode": 201,
        ///   "success": true,
        ///   "data": {
        ///     "Id": 1,
        ///     "ClassCode": "ABC123",
        ///     "StartDate": "2024-04-05T00:00:00",
        ///     "FinishDate": "2024-05-05T00:00:00",
        ///     "ClassName": "Mathematics",
        ///     "TeacherId": 1
        ///   }
        /// }
        /// </code>
        /// <para><b>Error Response:</b></para>
        /// <code>
        /// {
        ///   "message": "Request is invalid",
        ///   "success": false,
        ///   "errors": {
        ///     "StartDate": "Start date cannot be after finish date"
        ///   },
        ///   "data": null
        /// }
        /// </code>
        /// </remarks>
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

        /// <summary>
        /// Updates an existing teacher class.
        /// </summary>
        /// <remarks>
        /// <para>This endpoint allows you to update an existing teacher class by providing the class ID in the URL and the updated details in the request body.</para>
        /// <para><b>Example:</b></para>
        /// <code>
        /// curl -X PUT -k -H "Content-Type: application/json" -d '{"ClassCode": "DEF456", "StartDate": "2024-04-10", "FinishDate": "2024-05-10", "ClassName": "Science", "TeacherId": 2}' https://localhost:44302/api/classes/1
        /// </code>
        /// <para><b>Success Response:</b></para>
        /// <code>
        /// {
        ///   "message": "Teacher updated successfully",
        ///   "statusCode": 200,
        ///   "success": true,
        ///   "data": {
        ///     "Id": 1,
        ///     "ClassCode": "DEF456",
        ///     "StartDate": "2024-04-10T00:00:00",
        ///     "FormattedStartDate": "10/04/2024",
        ///     "FinishDate": "2024-05-10T00:00:00",
        ///     "FormattedFinishDate": "10/05/2024",
        ///     "ClassName": "Science",
        ///     "TeacherId": 2
        ///   }
        /// }
        /// </code>
        /// <para><b>Error Response:</b></para>
        /// <code>
        /// {
        ///   "message": "Request is invalid",
        ///   "success": false,
        ///   "errors": {
        ///     "teacherClass.ClassName": "Class name is required"
        ///   }
        /// }
        /// </code>
        /// </remarks>
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
