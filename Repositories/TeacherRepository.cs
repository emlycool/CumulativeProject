using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CumulativeProject.Models;
using MySql.Data.MySqlClient;

namespace CumulativeProject.Repositories
{
    public class TeacherRepository : BaseRepository
    {
        protected override string Table { get { return "teachers"; } }

        /// <summary>
        /// Retrieves a list of all teachers from the database.
        /// </summary>
        /// <returns>A list of all teachers.</returns>
        public new List<Teacher> All()
        {
            List <Dictionary<string, object>> results = base.All();
            return this.HydrateCollection(results);
        }

        /// <summary>
        /// Searches for teachers in the database based on a search query.
        /// </summary>
        /// <param name="searchQuery">The search query to filter teachers.</param>
        /// <returns>A list of teachers matching the search query.</returns>
        public List<Teacher> Search(string searchQuery)
        {
            // assign list to store teacher results
            List <Teacher> teachers = new List <Teacher>();

            string columns = "*";

            string query = $"SELECT {columns} FROM {this.Table} where CONCAT(teacherfname, ' ', teacherlname) like @searchQuery  or employeenumber like @searchQuery;";

            MySqlParameter[] parameters = {
                new MySqlParameter("@searchQuery", MySqlDbType.VarChar) { Value = "%" + searchQuery + "%" }
            };


            this.Query(
                query, 

                (MySqlDataReader reader, MySqlConnection Conn) =>
                {
                    // Read the data from the reader
                    while (reader.Read())
                    {
                        teachers.Add(this.HydrarteTeacherModel(reader));
                    }
                },
                parameters
            );

            return teachers;
        }

        /// <summary>
        /// Retrieves details of a specific teacher from the database.
        /// </summary>
        /// <param name="id">The ID of the teacher to retrieve.</param>
        /// <returns>The teacher details if found, otherwise null.</returns>
        public Teacher Find(int id)
        {
            string query = $"SELECT * FROM {this.Table} where teacherid = @id limit 1;";

            MySqlParameter[] parameters = {
                new MySqlParameter("@id", MySqlDbType.VarChar) { Value = id }
            };

            Teacher teacher = null;

            this.Query(
                query,
                (MySqlDataReader reader, MySqlConnection Conn) =>
                {
                    // Read the data from the reader
                    while (reader.Read())
                    {
                        teacher = this.HydrarteTeacherModel(reader);
                    }
                },
                parameters
            );

            return teacher;
        }


        public List<Teacher> GetTeachersByIds(List<int> teacherIds)
        {
            List<Teacher> teachers = new List<Teacher>();

            string query = $"SELECT * FROM {Table} WHERE teacherid IN ({string.Join(",", teacherIds.Select((_, index) => $"@id{index}"))})";

            List<MySqlParameter> parameters = teacherIds.Select((id, index) => new MySqlParameter($"@id{index}", id)).ToList();

            // Execute the query
            Query(
                query,
                (MySqlDataReader reader, MySqlConnection Conn) =>
                {
                    while (reader.Read())
                    {
                        Teacher teacher = this.HydrarteTeacherModel(reader);
                        teachers.Add(teacher);
                    }
                },
                parameters.ToArray()
            );

            return teachers;
        }


        public Teacher StoreTeacher(Teacher teacher)
        {
            Dictionary<string, object> userData = new Dictionary<string, object>();
            userData["teacherfname"] = teacher.FirstName;
            userData["teacherlname"] = teacher.LastName;
            userData["employeenumber"] = teacher.EmployeeNumber;
            userData["hiredate"] = teacher.HireDate;
            userData["salary"] = teacher.Salary;

            this.Insert(userData);

            teacher.Id = Convert.ToInt32(userData["id"]);
            return teacher;
        }

        public Teacher UpdateTeacher(Teacher teacher)
        {
            string query = $"UPDATE {this.Table} SET teacherfname = @teacherfname, teacherlname = @teacherlname, employeenumber = @employeenumber, hiredate = @hiredate, salary = @salary where teacherid = @teacherid;";


            int rowsAffected = this.ExecuteNonQuery(
                query,
                new MySqlParameter("@teacherfname", teacher.FirstName),
                new MySqlParameter("@teacherlname", teacher.LastName),
                new MySqlParameter("@employeenumber", teacher.EmployeeNumber),
                new MySqlParameter("@hiredate", teacher.HireDate),
                new MySqlParameter("@salary", teacher.Salary),
                new MySqlParameter("@teacherid", teacher.Id)
            );

            if( rowsAffected == 0)
            {
                throw new Exception("Failed to update Teacher");
            }
            return teacher;
        }

        public void DeleteTeacher(int id)
        {
            this.ClassRepository().UnassignedTeacherFromClasses(id);
            this.Delete(id, "teacherid");
        }
        
        /// <summary>
        /// Hydrates a list of dictionaries representing teacher data into a list of Teacher objects.
        /// </summary>
        /// <param name="results">The list of dictionaries representing teacher data.</param>
        /// <returns>A list of Teacher objects.</returns>
        protected List<Teacher> HydrateCollection(List<Dictionary<string, object>> results)
        {
            List<Teacher> teachers = new List<Teacher>();

            foreach (var item in results)
            {
                teachers.Add(this.HydrarteTeacherModel(item));
            }
            return teachers;
        }

        private ClassRepository ClassRepository()
        {
            return new ClassRepository();
        }

        public Teacher HydrarteTeacherModel(Dictionary<string, object> reader)
        {
            Teacher teacher = null;
            if (reader["teacherid"] != DBNull.Value)
            {
                teacher = new Teacher();
                teacher.Id = Convert.ToInt32(reader["teacherid"]);
                teacher.FirstName = Convert.ToString(reader["teacherfname"]);
                teacher.LastName = Convert.ToString(reader["teacherlname"]);
                teacher.EmployeeNumber = Convert.ToString(reader["employeenumber"]);
                teacher.HireDate = Convert.ToDateTime(reader["hiredate"]);
                teacher.Salary = Convert.ToDecimal(reader["salary"]);

            }

            return teacher;
        }

        public Teacher HydrarteTeacherModel(MySqlDataReader reader)
        {
            Teacher teacher = null;
            if (reader["teacherid"] != DBNull.Value)
            {
                teacher = new Teacher();
                teacher.Id = Convert.ToInt32(reader["teacherid"]);
                teacher.FirstName = Convert.ToString(reader["teacherfname"]);
                teacher.LastName = Convert.ToString(reader["teacherlname"]);
                teacher.EmployeeNumber = Convert.ToString(reader["employeenumber"]);
                teacher.HireDate = Convert.ToDateTime(reader["hiredate"]);
                teacher.Salary = Convert.ToDecimal(reader["salary"]);

            }

            return teacher;
        }
    }
}
