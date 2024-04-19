using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using CumulativeProject.Models;
using MySql.Data.MySqlClient;

namespace CumulativeProject.Repositories
{
    public class ClassRepository : BaseRepository
    {
        protected override string Table { get { return "classes"; } }

        public List<TeacherClass> GetTeacherClasses(int teacherId)
        {
            List<TeacherClass> classes = new List<TeacherClass>();

            string query = $"select * from {this.Table} where teacherid = @teacherid";

            this.Query(
                query,
                (MySqlDataReader reader, MySqlConnection Conn) =>
                {
                    while (reader.Read())
                    {
                        classes.Add(new TeacherClass
                        {
                            Id = Convert.ToInt32(reader["classid"]),
                            ClassCode = Convert.ToString(reader["classcode"]),
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            ClassName = Convert.ToString(reader["classname"]),
                            StartDate = Convert.ToDateTime(reader["startdate"]),
                            FinishDate = Convert.ToDateTime(reader["finishdate"])
                        });
                    }
                },
                parameters: new MySqlParameter("@teacherid", teacherId)
            );

            return classes;
        }

        /// <summary>
        /// Hydrates a list of dictionaries representing teacher data into a list of Teacher objects.
        /// </summary>
        /// <param name="results">The list of dictionaries representing teacher data.</param>
        /// <returns>A list of Teacher objects.</returns>
        protected List<TeacherClass> HydrateCollection(List<Dictionary<string, object>> results)
        {
            List<TeacherClass> teacherClasses = new List<TeacherClass>();

            foreach (var item in results)
            {
                teacherClasses.Add(new TeacherClass
                {
                    Id = Convert.ToInt32(item["classid"]),
                    ClassCode = Convert.ToString(item["classcode"]),
                    TeacherId = item["teacherid"] != DBNull.Value ? Convert.ToInt32(item["teacherid"]) : (int?)null,
                    ClassName = Convert.ToString(item["classname"]),
                    StartDate = Convert.ToDateTime(item["startdate"]),
                    FinishDate = Convert.ToDateTime(item["finishdate"])
                });
            }
            return teacherClasses;
        }

        public void UnassignedTeacherFromClasses(int id)
        {
            this.ExecuteNonQuery(
                $"UPDATE {this.Table} SET teacherid = null where teacherid = @id;",
                new MySqlParameter("@id", id)
            );
        }

        /// <summary>
        /// Searches for classes in the database based on a search query.
        /// </summary>
        /// <param name="searchQuery">The search query to filter classes.</param>
        /// <returns>A list of class matching the search query.</returns>
        public List<TeacherClass> Search(string searchQuery)
        {
            // assign list to store classes results
            List<TeacherClass> classes = new List<TeacherClass>();

            string columns = "*";

            string query = $"SELECT {columns} FROM {this.Table} left join teachers on classes.teacherid = teachers.teacherid where classes.classname like @searchQuery  or classes.classcode like @searchQuery;";

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
                        Teacher teacher = null;
                        if (reader["teacherid"] != DBNull.Value)
                        {
                            teacher = this.TeacherRepository().HydrarteTeacherModel(reader);
                            
                        }
                        classes.Add(new TeacherClass
                        {
                            Id = Convert.ToInt32(reader["classid"]),
                            ClassCode = Convert.ToString(reader["classcode"]),
                            TeacherId = reader["teacherid"] != DBNull.Value ? Convert.ToInt32(reader["teacherid"]) : (int?)null,
                            ClassName = Convert.ToString(reader["classname"]),
                            StartDate = Convert.ToDateTime(reader["startdate"]),
                            FinishDate = Convert.ToDateTime(reader["finishdate"]),
                            Teacher = teacher
                        });
                    }
                },
                parameters
            );

            return classes;
        }

        /// <summary>
        /// Retrieves a list of all classes from the database.
        /// </summary>
        /// <returns>A list of all classes.</returns>
        public new List<TeacherClass> All()
        {
            List<Dictionary<string, object>> results = base.All();

            List<TeacherClass> teacherClasses = this.HydrateCollection(results);

            List<int> teacherIds = teacherClasses.Where(d => d.TeacherId != null)
                .Select(d => (int)d.TeacherId)
                .ToList();

            List<Teacher> teachers = this.TeacherRepository().GetTeachersByIds(teacherIds);


            foreach (TeacherClass teacherClass in teacherClasses)
            {
                teacherClass.Teacher = teachers.FirstOrDefault(t => t.Id == teacherClass.TeacherId);
            }

            return teacherClasses;
        }

        /// <summary>
        /// Retrieves details of a specific class from the database.
        /// </summary>
        /// <param name="id">The ID of the class to retrieve.</param>
        /// <returns>The class details if found, otherwise null.</returns>
        public TeacherClass Find(int id)
        {
            Debug.WriteLine(id);

            string query = $"SELECT * FROM {this.Table} where classid = @id limit 1;";

            MySqlParameter[] parameters = {
                new MySqlParameter("@id", MySqlDbType.VarChar) { Value = id }
            };

            TeacherClass teacherClass = null;

            this.Query(
                query,
                (MySqlDataReader reader, MySqlConnection Conn) =>
                {
                    // Read the data from the reader
                    while (reader.Read())
                    {
                        teacherClass = this.HydrarteClassModel(reader);
                    }
                },
                parameters
            );

            this.LoadTeacher(teacherClass);

            return teacherClass;
        }

        public TeacherClass StoreClass(TeacherClass teacherClass)
        {
            Dictionary<string, object> userData = new Dictionary<string, object>();
            userData["classname"] = teacherClass.ClassName;
            userData["classcode"] = teacherClass.ClassCode;
            userData["finishdate"] = teacherClass.FinishDate;
            userData["startdate"] = teacherClass.StartDate;
            userData["teacherid"] = teacherClass.TeacherId;

            this.Insert(userData);

            teacherClass.Id = Convert.ToInt32(userData["id"]);
            this.LoadTeacher(teacherClass);
            return teacherClass;
        }

        public void DeleteClass(int id)
        {
            this.Delete(id, "classid");
        }

        public TeacherClass UpdateClass(TeacherClass TeacherClass)
        {
            string query = $"UPDATE {this.Table} SET classname = @classname, classcode = @classcode, startdate = @startdate, finishdate = @finishdate, teacherid = @teacherid where classid = @classid;";


            int rowsAffected = this.ExecuteNonQuery(
                query,
                new MySqlParameter("@classname", TeacherClass.ClassName),
                new MySqlParameter("@classcode", TeacherClass.ClassCode),
                new MySqlParameter("@startdate", TeacherClass.StartDate),
                new MySqlParameter("@finishdate", TeacherClass.FinishDate),
                new MySqlParameter("@teacherid", TeacherClass.TeacherId),
                new MySqlParameter("@classid", TeacherClass.Id)
            );

            if (rowsAffected == 0)
            {
                throw new Exception("Failed to update Teacher");
            }
            return TeacherClass;
        }

        private void LoadTeacher(TeacherClass teacherClass)
        {
            if (teacherClass?.TeacherId != null)
            {
                teacherClass.Teacher = this.TeacherRepository().Find((int)teacherClass.TeacherId);
            }
        }

        private TeacherRepository TeacherRepository()
        {
            return new TeacherRepository();
        }

        public TeacherClass HydrarteClassModel(MySqlDataReader reader)
        {
            TeacherClass teacherClass = new TeacherClass
            {
                Id = Convert.ToInt32(reader["classid"]),
                ClassCode = Convert.ToString(reader["classcode"]),
                TeacherId = reader["teacherid"] != DBNull.Value ? Convert.ToInt32(reader["teacherid"]) : (int?)null,
                ClassName = Convert.ToString(reader["classname"]),
                StartDate = Convert.ToDateTime(reader["startdate"]),
                FinishDate = Convert.ToDateTime(reader["finishdate"])
            };

            return teacherClass;
        }
    }
}