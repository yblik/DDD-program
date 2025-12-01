using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal class SQLmanager
    {
        public static bool RecordExists(string tableName, string columnName, string value)
        {
            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();
                string query = $"SELECT COUNT(1) FROM {tableName} WHERE {columnName} = @value;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@value", value);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public static bool AddUserWithDetails(
            string username,
            string password,
            string role,
            string name,
            int age,
            int year,
            string feeling = "",
            string ailments = "",
            int hiddenAilments = 0)
        {
            if (RecordExists("Users", "Username", username))
            {
                Console.WriteLine($"User '{username}' already exists.");
                return false;
            }

            try
            {
                using (var connection = SQLstorage.GetConnection())
                {
                    connection.Open();

                    // Users table
                    string queryUsers = @"
                        INSERT INTO Users (Username, Password, Role, Age)
                        VALUES (@username, @password, @role, @age);";
                    using (var command = new SQLiteCommand(queryUsers, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@age", age);
                        command.ExecuteNonQuery();
                    }

                    // Profiles table
                    string queryProfiles = @"
                        INSERT INTO Profiles (Username, Name, Age, Year)
                        VALUES (@username, @name, @age, @year);";
                    using (var command = new SQLiteCommand(queryProfiles, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@age", age);
                        command.Parameters.AddWithValue("@year", year);
                        command.ExecuteNonQuery();
                    }

                    // HealthSupport table (students only)
                    if (role.ToLower() == "student")
                    {
                        string queryHealth = @"
                            INSERT INTO HealthSupport (Username, Feeling, Ailments, HiddenAilments)
                            VALUES (@username, @feeling, @ailments, @hiddenAilments);";
                        using (var command = new SQLiteCommand(queryHealth, connection))
                        {
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@feeling", feeling);
                            command.Parameters.AddWithValue("@ailments", ailments);
                            command.Parameters.AddWithValue("@hiddenAilments", hiddenAilments);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                Console.WriteLine($"User '{username}' successfully created.");
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite Error: {ex.Message}");
                return false;
            }
        }

        public static void AddLog(string username, string role, string action)
        {
            try
            {
                using (var connection = SQLstorage.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Logs (Username, Role, LoginTime, ActionsNoted)
                        VALUES (@username, @role, @loginTime, @actions);";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@loginTime", DateTime.Now.ToString("s"));
                        command.Parameters.AddWithValue("@actions", action);
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine($"Log added for {username}: {action}");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite Error (Log): {ex.Message}");
            }
        }


        /// <summary>
        /// TO GET ASSIGNMENT WORKING
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        // NEW: Get users by role for dropdown


        public static List<string> GetUsernamesByRole(string role)
        {
            var usernames = new List<string>();

            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();
                string query = "SELECT Username FROM Users WHERE Role = @role ORDER BY Username";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@role", role);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usernames.Add(reader["Username"].ToString());
                        }
                    }
                }
            }
            return usernames;
        }

        // Method 2: Update the fucking table
        public static bool AssignStudentToSupervisor(string studentUsername, string supervisorUsername)
        {
            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();

                // Try to update. If no row is updated, then insert.
                string updateSql = "UPDATE Profiles SET Assigned = @supervisor WHERE Username = @student";
                using (var updateCmd = new SQLiteCommand(updateSql, connection))
                {
                    updateCmd.Parameters.AddWithValue("@supervisor", supervisorUsername);
                    updateCmd.Parameters.AddWithValue("@student", studentUsername);
                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return true;
                    }
                }

                // If no row was updated, then insert a new row.
                string insertSql = @"
            INSERT INTO Profiles (Username, Name, Age, Year, Assigned)
            VALUES (@student, NULL, NULL, NULL, @supervisor)";
                using (var insertCmd = new SQLiteCommand(insertSql, connection))
                {
                    insertCmd.Parameters.AddWithValue("@student", studentUsername);
                    insertCmd.Parameters.AddWithValue("@supervisor", supervisorUsername);
                    insertCmd.ExecuteNonQuery();
                }
                return true;
            }
        }
        public static List<(string Username, string Name)> GetUsersByRole(string role)
        {
            var users = new List<(string, string)>();

            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();
                string query = @"
                SELECT u.Username, p.Name 
                FROM Users u 
                JOIN Profiles p ON u.Username = p.Username 
                WHERE u.Role = @role
                ORDER BY p.Name";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@role", role);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add((reader["Username"].ToString(), reader["Name"].ToString()));
                        }
                    }
                }
            }
            return users;
        }

        // NEW: Get unassigned students
        public static List<(string Username, string Name)> GetUnassignedStudents()
        {
            var students = new List<(string, string)>();

            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();
                string query = @"
                SELECT u.Username, p.Name 
                FROM Users u 
                JOIN Profiles p ON u.Username = p.Username 
                WHERE u.Role = 'Student' 
                AND (p.Assigned IS NULL OR p.Assigned = '')
                ORDER BY p.Name";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add((reader["Username"].ToString(), reader["Name"].ToString()));
                        }
                    }
                }
            }
            return students;
        }

        // NEW: Get supervisor's current students
        public static List<(string Username, string Name)> GetSupervisorStudents(string supervisorUsername)
        {
            var students = new List<(string, string)>();

            using (var connection = SQLstorage.GetConnection())
            {
                connection.Open();
                string query = @"
                SELECT p.Username, p.Name 
                FROM Profiles p
                JOIN Users u ON p.Username = u.Username
                WHERE p.Assigned = @supervisorUsername
                AND u.Role = 'Student'
                ORDER BY p.Name";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@supervisorUsername", supervisorUsername);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add((reader["Username"].ToString(), reader["Name"].ToString()));
                        }
                    }
                }
            }
            return students;
        }
    }
}