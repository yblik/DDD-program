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
                connection.Close();
            }
        }

        private static void CreateTables(SQLiteConnection connection)
        {
            string usersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    Age INT,
                    Meetings TEXT DEFAULT '',
                    Availability TEXT DEFAULT ''
                );";

            string meetingsTable = @"
                CREATE TABLE IF NOT EXISTS Meetings (
                    MeetingID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Student TEXT,
                    Role TEXT,
                    MeetingDate TEXT,
                    Details TEXT,
                    Supervisor TEXT,
                    Accepted BOOLEAN
                );";

            string profilesTable = @"
                CREATE TABLE IF NOT EXISTS Profiles (
                    Username TEXT PRIMARY KEY,
                    Name TEXT,
                    Age INT,
                    Year INT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

            string healthTable = @"
                CREATE TABLE IF NOT EXISTS HealthSupport (
                    Username TEXT PRIMARY KEY,
                    Feeling TEXT,
                    Ailments TEXT,
                    HiddenAilments TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

            string reportsTable = @"
                CREATE TABLE IF NOT EXISTS Reports (
                    ReportID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT,
                    Respondent TEXT,
                    Role TEXT,
                    ReportType TEXT,
                    Detail TEXT,
                    FOREIGN KEY(Username) REFERENCES Users(Username)
                );";

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

            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = usersTable; command.ExecuteNonQuery();
                command.CommandText = meetingsTable; command.ExecuteNonQuery();
                command.CommandText = profilesTable; command.ExecuteNonQuery();
                command.CommandText = healthTable; command.ExecuteNonQuery();
                command.CommandText = reportsTable; command.ExecuteNonQuery();
                command.CommandText = logsTable; command.ExecuteNonQuery();
            }

            Console.WriteLine("All tables verified or created.");
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }

    
}
