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
        /// Adds a new user to the Users, Profiles, and HealthSupport tables.
        /// </summary>
        /// <param name="username">Username of the new user</param>
        /// <param name="password">Password of the new user</param>
        /// <param name="role">Role of the user (e.g., Student, Supervisor, Senior Tutor).</param>
        /// <param name="name">Full name of the user</param>
        /// <param name="age">Age of the user</param>
        /// <param name="year">Academic year (students) or blank for staff</param>
        /// <param name="feeling">Current feeling (students only)</param>
        /// <param name="ailments">Known ailments (students only)</param>
        /// <param name="hiddenAilments">Hidden ailments (students only)</param>
        /// <returns>True if user was added; false if user already exists</returns>
        public static bool AddUserWithDetails(
            string username,
            string password,
            string role,
            string name,
            int year,
            string feeling = "",
            string ailments = "",
            string hiddenAilments = "")
        {
            // Step 1: Check if username already exists
            if (RecordExists("Users", "Username", username))
            {
                Console.WriteLine($"User '{username}' already exists. Using existing record.");
                return false;
            }

            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();

                // Step 2: Insert into Users table
                string queryUsers = @"
                    INSERT INTO Users (Username, Password, Role, Meetings, Availability)
                    VALUES (@username, @password, @role, '', '');";

                using (var commandUsers = new SQLiteCommand(queryUsers, connection))
                {
                    commandUsers.Parameters.AddWithValue("@username", username);
                    commandUsers.Parameters.AddWithValue("@password", password);
                    commandUsers.Parameters.AddWithValue("@role", role);
                    commandUsers.ExecuteNonQuery();
                }

                // Step 3: Insert into Profiles table
                string queryProfile = @"
                    INSERT INTO Profiles (Username, Name, Age, Year)
                    VALUES (@username, @name, @age, @year);";

                using (var commandProfile = new SQLiteCommand(queryProfile, connection))
                {
                    commandProfile.Parameters.AddWithValue("@username", username);
                    commandProfile.Parameters.AddWithValue("@name", name);
                    commandProfile.Parameters.AddWithValue("@age", age);
                    commandProfile.Parameters.AddWithValue("@year", year);
                    commandProfile.ExecuteNonQuery();
                }

                // Step 4: Insert into HealthSupport table (only for students)
                if (role.ToLower() == "student")
                {
                    string queryHealth = @"
                        INSERT INTO HealthSupport (Username, Feeling, Ailments, HiddenAilments)
                        VALUES (@username, @feeling, @ailments, @hiddenAilments);";

                    using (var commandHealth = new SQLiteCommand(queryHealth, connection))
                    {
                        commandHealth.Parameters.AddWithValue("@username", username);
                        commandHealth.Parameters.AddWithValue("@feeling", feeling);
                        commandHealth.Parameters.AddWithValue("@ailments", ailments);
                        commandHealth.Parameters.AddWithValue("@hiddenAilments", hiddenAilments);
                        commandHealth.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine($"User '{username}' successfully created with full profile.");
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
