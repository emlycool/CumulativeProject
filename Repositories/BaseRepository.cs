using System;
using System.Collections.Generic;
using CumulativeProject.Data;
using CumulativeProject.Interfaces;
using MySql.Data.MySqlClient;

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
        public void Query(string query, Action<MySqlDataReader, MySqlConnection> closure, params MySqlParameter[] parameters)
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

                closure(ResultSet, Conn);

                Conn.Close();
            }
        }

        /// <summary>
        /// Retrieves all rows from the table.
        /// </summary>
        /// <param name="selected">Optional array of column names to select.</param>
        /// <returns>A list of dictionaries containing column name-value pairs for each row.</returns>
        public List<Dictionary<string, object>> All(string[] selected = null)
        {
            // Check if selected is null and assign the empty array if needed
            if (selected == null)
            {
                selected = new string[0];
            }

            // Initialize a list to store the rows
            var rows = new List<Dictionary<string, object>>();

            string columns = selected.Length > 0 ? string.Join(", ", selected) : "*";
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
