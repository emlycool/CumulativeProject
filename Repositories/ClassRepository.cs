using System;
using System.Collections.Generic;
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
                    TeacherId = Convert.ToInt32(item["teacherid"]),
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
    }
}