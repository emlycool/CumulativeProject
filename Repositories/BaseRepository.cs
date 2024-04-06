using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using CumulativeProject.Data;
using CumulativeProject.Interfaces;
using MySql.Data.MySqlClient;
using Sprache;

namespace CumulativeProject.Repositories
{
    public abstract class BaseRepository
    {
        protected DbContextInterface dbContext;

        protected BaseRepository()
        {
            this.dbContext = new SchoolDbContext();
        }

        /// <summary>
        /// Gets the name of the table storing teacher data in the database.
        /// </summary>
        protected abstract string Table { get; }

        /// <summary>
        /// Executes a SQL query against the database and performs the specified action on the result set.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="closure">The action to perform on the result set.</param>
        /// <param name="parameters">Optional parameters to include in the query.</param>
        public void Query(string query, Action<MySqlDataReader, MySqlConnection> closure = null, params MySqlParameter[] parameters)
        {
            // Use try-with-resources to automatically close connection
            using (MySqlConnection Conn = this.dbContext.AccessDatabase())
            {
                // Open the connection between the web server and database
                Conn.Open();

                // Establish a new command (query) for our database
                MySqlCommand cmd = Conn.CreateCommand();

                // SQL QUERY
                cmd.CommandText = query;

                // Add parameters to the command
                foreach (MySqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                // Gather Result Set of Query into a variable
                MySqlDataReader ResultSet = cmd.ExecuteReader();

                if (closure != null)
                {
                    closure(ResultSet, Conn);
                }


                Conn.Close();
            }
        }

        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection Conn = this.dbContext.AccessDatabase())
            {
                Conn.Open();
                int result;
                using (MySqlCommand command = new MySqlCommand(query, Conn))
                {
                    command.Parameters.AddRange(parameters);
                    result = command.ExecuteNonQuery();
                }
                Conn.Close();

                return result;
            }
        }

        /// <summary>
        /// Inserts data into the table.
        /// </summary>
        /// <param name="values">A dictionary containing column name-value pairs for the row to be inserted.</param>
        public void Insert(Dictionary<string, object> values)
        {
            string columns = string.Join(", ", values.Keys);
            string parameters = string.Join(", ", values.Keys.Select(key => "@" + key));
            string query = $"INSERT INTO {this.Table} ({columns}) VALUES ({parameters});";

            //Debug.WriteLine(query);

            List<MySqlParameter> parametersList = new List<MySqlParameter>();

            foreach (var pair in values)
            {
                //Debug.WriteLine($" @{pair.Key} @{pair.Value}");
                parametersList.Add(new MySqlParameter($"@{pair.Key}", pair.Value));
            }

            int rowsAffected = this.ExecuteNonQuery(query, parameters: parametersList.ToArray());
            if(rowsAffected == 0)
            {
                throw new Exception("Failed to insert record");
            }

            string selectQuery = "SELECT LAST_INSERT_ID();";
            this.Query(selectQuery, (reader, connection) => {
                if (reader.Read())
                {
                    values["id"] = Convert.ToInt32(reader[0]);
                }
            });
        }

        public void Delete(int id, string column = null)
        {
            column = column ?? "id";
            string queryCommand = $"Delete from {this.Table} where {column} = @id";
            MySqlParameter paramter = new MySqlParameter("@id", id);
            
            this.ExecuteNonQuery(queryCommand, parameters: paramter);

        }

        /// <summary>
        /// Retrieves all rows from the table.
        /// </summary>
        /// <returns>A list of dictionaries containing column name-value pairs for each row.</returns>
        public List<Dictionary<string, object>> All()
        {
            // Initialize a list to store the rows
            var rows = new List<Dictionary<string, object>>();

            string columns = "*";
            this.Query($"SELECT {columns} FROM {this.Table}", (MySqlDataReader reader, MySqlConnection Conn) =>
            {
                // Read the data from the reader
                while (reader.Read())
                {
                    // Create a dictionary to store the column name-value pairs for each row
                    var row = new Dictionary<string, object>();

                    // Populate the columns for this row
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        row[columnName] = reader.GetValue(i);
                    }

                    // Add the row to the list
                    rows.Add(row);
                }

            });

            return rows;
        }

    }
}
