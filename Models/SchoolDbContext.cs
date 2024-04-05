using System;
using System.Diagnostics;
using CumulativeProject.Interfaces;
using MySql.Data.MySqlClient;

namespace CumulativeProject.Data
{
    public class SchoolDbContext : DbContextInterface
    {
        //These are readonly "secret" properties.
        //Only the BlogDbContext class can use them.
        //Change these to match your own local blog database!
        private static string User { get { return "root"; } }
        private static string Password { get { return ""; } }
        private static string Database { get { return "school_db"; } }
        private static string Server { get { return "localhost"; } }
        private static string Port { get { return "3306"; } }

        //ConnectionString is a series of credentials used to connect to the database.
        protected static string ConnectionString
        {
            get
            {
                //convert zero datetime is a db connection setting which returns NULL if the date is 0000-00-00
                //this can allow C# to have an easier interpretation of the date (no date instead of 0 BCE)
                string server = DotNetEnv.Env.GetString("DbServer", "localhost");
                string user = DotNetEnv.Env.GetString("DbUser", "root");
                string password = DotNetEnv.Env.GetString("DbPassword", "");
                string database = DotNetEnv.Env.GetString("DbName");
                string port = DotNetEnv.Env.GetString("DbPort", "3306");

                // Build the connection string
                string connectionString = $"server={server};user={user};database={database};port={port};password={password};convert zero datetime=True";

                return connectionString;
            }
        }
        //This is the method we actually use to get the database!
        /// <summary>
        /// Returns a connection to the blog database.
        /// </summary>
        /// <example>
        /// private BlogDbContext Blog = new BlogDbContext();
        /// MySqlConnection Conn = Blog.AccessDatabase();
        /// </example>
        /// <returns>A MySqlConnection Object</returns>
        public MySqlConnection AccessDatabase()
        {
            //We are instantiating the MySqlConnection Class to create an object
            //the object is a specific connection to our blog database on port 3307 of localhost
            return new MySqlConnection(ConnectionString);
        }
    }
}
