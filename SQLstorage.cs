using System;
using System.Data.SQLite;
using System.IO;

namespace DDD_program
{
    internal static class SQLstorage
    {
        private const string DatabaseFile = "SqlSystem.db";
        private const string ConnectionString = "Data Source=" + DatabaseFile + ";Version=3;";

        public static void Initialize()
        {
            bool newDatabase = !File.Exists(DatabaseFile);

            if (newDatabase)
            {
                SQLiteConnection.CreateFile(DatabaseFile);
                Console.WriteLine("Database file created.");
            }
            else
            {
                Console.WriteLine("Database file found. Using existing database.");
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                CreateTables(connection);

                if (newDatabase)
                {
                    InsertDefaultData(connection);
                }

                connection.Close();
            }
        }

        private static void CreateTables(SQLiteConnection connection)
        {
            string[] tableScripts = {
                @"CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    Age INTEGER,
                    Meetings TEXT DEFAULT '',
                    Availability TEXT DEFAULT ''
                );",

                @"CREATE TABLE IF NOT EXISTS Meetings (
                    MeetingID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Student TEXT,
                    Role TEXT,
                    MeetingDate TEXT,
                    Details TEXT,
                    Supervisor TEXT,
                    FromStudent BOOLEAN DEFAULT 0,
                    Accepted BOOLEAN DEFAULT 0
                );",

                @"CREATE TABLE IF NOT EXISTS Profiles (
                    Username TEXT PRIMARY KEY,
                    Name TEXT,
                    Age INTEGER,
                    Year INTEGER,
                    FOREIGN KEY(Username) REFERENCES Users(Username) ON DELETE CASCADE
                );",

                @"CREATE TABLE IF NOT EXISTS HealthSupport (
                    Username TEXT PRIMARY KEY,
                    Feeling TEXT DEFAULT '',
                    Ailments TEXT DEFAULT '',
                    HiddenAilments INTEGER,
                    FOREIGN KEY(Username) REFERENCES Users(Username) ON DELETE CASCADE
                );",

                @"CREATE TABLE IF NOT EXISTS Reports (
                    ReportID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT,
                    Respondent TEXT,
                    Role TEXT,
                    ReportType TEXT,
                    Detail TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username) ON DELETE CASCADE
                );",

                @"CREATE TABLE IF NOT EXISTS Logs (
                    LogID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT,
                    Role TEXT,
                    LoginTime TEXT,
                    LogoutTime TEXT,
                    ActionsNoted TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username) ON DELETE CASCADE
                );"
            };

            using var command = new SQLiteCommand(connection);
            foreach (var script in tableScripts)
            {
                command.CommandText = script;
                command.ExecuteNonQuery();
            }

            Console.WriteLine("All tables verified or created.");
        }

        private static void InsertDefaultData(SQLiteConnection connection)
        {
            // Insert some default users for testing
            string[] defaultUsers = {
                "INSERT INTO Users (Username, Password, Role, Age) VALUES ('student1', 'pass', 'student', 20);",
                "INSERT INTO Users (Username, Password, Role, Age) VALUES ('supervisor1', 'pass', 'supervisor', 35);",
                "INSERT INTO Profiles (Username, Name, Age, Year) VALUES ('student1', 'John Doe', 20, 2);",
                "INSERT INTO HealthSupport (Username, Feeling, Ailments, HiddenAilments) VALUES ('student1', 'Good', 'None', 1);"
            };

            using var command = new SQLiteCommand(connection);
            foreach (var sql in defaultUsers)
            {
                try
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not insert default data: {ex.Message}");
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}