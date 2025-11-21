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
            string hiddenAilments = "")
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
    }
}
