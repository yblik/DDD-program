using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    /// <summary>
    /// Handles CRUD operations for the db - note in docs
    /// 
    /// Responsibilities include:
    /// - Checking if a record already exists
    /// - Adding new users
    /// - Logging user actions
    /// </summary>
    internal class SQLmanager
    {
        /// <summary>
        /// Checks whether a record exists in a specified table and column
        /// </summary>
        /// <param name="tableName">The name of table to query</param>
        /// <param name="columnName">The column to check for the value</param>
        /// <param name="value">The value to look for in the column</param>
        /// <returns>True if the record exists; false otherwise.</returns>
        /// 
        //open connection to db -> insert 
        public static bool RecordExists(string tableName, string columnName, string value)
        {
            // Open a connection to the SQLite db
            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();

                // SQL query to count how many records match the given value
                string query = $"SELECT COUNT(1) FROM {tableName} WHERE {columnName} = @value;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    // Parameterize the value to prevent SQL injection
                    command.Parameters.AddWithValue("@value", value);

                    // Execute the query and return true if at least one record exists
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Adds a new user to the Users table, if they do not already exist.
        /// </summary>
        /// <param name="username">Username of the new user</param>
        /// <param name="password">Password of the new user</param>
        /// <param name="role">Role of the user (e.g., Student, Supervisor, Senior Tutor).</param>
        /// <returns>True if user was added; false if user already exists</returns>
        public static bool AddUser(string username, string password, string role)
        {
            // First check if a user with the same username already exists so no repeats
            if (RecordExists("Users", "Username", username))
            {
                Console.WriteLine($"User '{username}' already exists. Using existing record.");
                return false;
            }

            // If user doesn't exist, open a connection to insert the new record
            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();

                // Parameterized SQL insert to add the new user
                string query = @"
                    INSERT INTO Users (Username, Password, Role)
                    VALUES (@username, @password, @role);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@role", role);
                    command.ExecuteNonQuery(); // Execute the insert command
                }
            }

            Console.WriteLine($"User '{username}' added successfully.");
            return true;
        }

        /// <summary>
        /// Adds a log entry to the Logs table for tracking user actions.
        /// </summary>
        /// <param name="username">The username of the user performing the action.</param>
        /// <param name="role">The role of the user (Student, Supervisor, etc.).</param>
        /// <param name="action">Description of the action performed.</param>
        public static void AddLog(string username, string role, string action)
        {
            // Open a connection to the database
            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();

                // Insert a new log entry
                string query = @"
                    INSERT INTO Logs (Username, Role, LoginTime, ActionsNoted)
                    VALUES (@username, @role, @loginTime, @actions);";

                //AI stuff here
                using (var command = new SQLiteCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@role", role);
                    command.Parameters.AddWithValue("@loginTime", DateTime.Now.ToString("s")); // ISO 8601 timestamp
                    command.Parameters.AddWithValue("@actions", action);

                    // Execute the insert
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Log added for {username}: {action}");
        }
    }
}
