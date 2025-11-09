using System;
using System.Data.SQLite;
using System.IO;

namespace DDD_program
{
    internal static class SQLstorage
    {
        //stores username column and passwords, connecting to table with calender dates and the type (supervisor meeting, or senior or absence)
        //another table for username storing student profile (username, name, age, acedemic year, ailments[], hidden ailments[])

        //S.Sup.ST.table 1 = username, role, password, meetings, availability
        //S.Sup.ST.table 2 (detail) = username, name, age, year
        //S.table 3 (health&support) = username, feelings, ailments[], hiddenAilments[]
        //student profile = table2+table3
        //S.Sup.table 4 (report) = username, role, report type, detail - later
        //S.Sup.table 5 (log) = username, role, login time, logout time, actionsNoted[] - later

        /// <summary>
        /// Handles all SQLite database setup and connection logic for the calendar system.
        /// This class is responsible for:
        /// - Creating the database file (if it doesn't already exist)
        /// - Setting up required tables
        /// - Providing a method to get a connection to the database
        /// </summary>
        /// 
        // The filename for the SQLite database.
        private const string DatabaseFile = "SqlSystem.db";

        // The connection string tells SQLite where the database file is and what version to use.
        private const string ConnectionString = "Data Source=" + DatabaseFile + ";Version=3;";

            /// <summary>
            /// Initializes the database system. 
            /// Checks if the database file exists; if not, creates it and sets up the tables.
            /// </summary>
        public static void Initialize()
        {
                // Check if the database file already exists in the proj folder
                bool newDatabase = !File.Exists(DatabaseFile);

                if (newDatabase)
                {
                    // If not found create a new database file
                    SQLiteConnection.CreateFile(DatabaseFile);
                    Console.WriteLine("Database file created.");
                }
                else
                {
                    // If found notify
                    Console.WriteLine("Database file found. Using existing database.");
                }

                // Open a connection to the db so can run table setup commands
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    CreateTables(connection); // Ensure all required tables exist.
                    connection.Close();
                }
        }

            /// <summary>
            /// Creates tables for system if they don't already exist
            /// </summary>
            /// <param name="connection">An open SQLlite to the db</param>
        private static void CreateTables(SQLiteConnection connection)
        {
                // Table 1: User login and core data
                string usersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Username TEXT PRIMARY KEY,
                        Password TEXT NOT NULL,
                        Role TEXT NOT NULL,
                );";
                // Table 2: Meetings 
                string meetingTable = @"
                    CREATE TABLE IF NOT EXISTS Meetings (
                    Student TEXT PRIMARY KEY,
                    Role TEXT,
                    MeetingDate TEXT,
                    Details TEXT,
                    Supervisor TEXT,
                );";

            // Table 3: Basic user profile information (linked to Users)
            string profilesTable = @"
                CREATE TABLE IF NOT EXISTS Profiles (
                    Username TEXT PRIMARY KEY,
                    Name TEXT,
                    Year INT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

                // Table 4: Student health and support information (linked to Users)
                string healthTable = @"
                CREATE TABLE IF NOT EXISTS HealthSupport (
                    Username TEXT PRIMARY KEY,
                    Feeling TEXT,
                    Ailments TEXT,
                    HiddenAilments TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

                // Table 5: Report records (students or staff can create reports)
                string reportsTable = @"
                CREATE TABLE IF NOT EXISTS Reports (
                    ReportID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT,
                    Role TEXT,
                    ReportType TEXT,
                    Detail TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

                // Table 6: System activity logs (tracks user actions)
                string logsTable = @"
                CREATE TABLE IF NOT EXISTS Logs (
                    LogID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT,
                    Role TEXT,
                    LoginTime TEXT,
                    LogoutTime TEXT,
                    ActionsNoted TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

                // Execute each CREATE TABLE command in sequence.
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = usersTable; command.ExecuteNonQuery();
                    command.CommandText = meetingTable; command.ExecuteNonQuery();
                    command.CommandText = profilesTable; command.ExecuteNonQuery();
                    command.CommandText = healthTable; command.ExecuteNonQuery();
                    command.CommandText = reportsTable; command.ExecuteNonQuery();
                    command.CommandText = logsTable; command.ExecuteNonQuery();
                }

                Console.WriteLine("All tables verified or created.");
        }

            /// <summary>
            /// Provides a usable SQLlite object.
            /// Use in other classes (like data managers) to run queries or insert data.
            /// </summary>
            /// <returns>A new SQLlite using the stored connection string.</returns>
            public static SQLiteConnection GetConnection()
            {
                return new SQLiteConnection(ConnectionString);
            }
        }
    }