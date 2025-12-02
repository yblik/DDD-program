using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class SystemLogViewerMenuItem : MenuItem
    {
        private readonly int Role;           // Should only work for Role = 3
        private readonly string Username;    // Senior tutor username

        public SystemLogViewerMenuItem(int role, string username)
        {
            Role = role;
            Username = username;
        }

        public override string MenuText()
        {
            return "View System Logs (Senior Tutor Only)";
        }

        public override void Select()
        {
            if (Role != 3)
            {
                Console.WriteLine("You do not have permission to access system logs.");
                Console.ReadLine();
                return;
            }

            SystemLogger.LogAction("Viewed system logs");

            Console.Clear();
            Console.WriteLine("=== SYSTEM LOGS ===\n");

            var logs = LoadLogs();

            if (logs.Count == 0)
            {
                Console.WriteLine("No logs available.");
                Console.ReadLine();
                return;
            }

            foreach (var log in logs)
            {
                Console.WriteLine($"Log ID: {log.LogID}");
                Console.WriteLine($"User: {log.Username}");
                Console.WriteLine($"Role: {log.Role}");
                Console.WriteLine($"Login: {log.LoginTime}");
                Console.WriteLine($"Logout: {log.LogoutTime}");
                Console.WriteLine($"Actions: {log.ActionsNoted}");
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("Press ENTER to return...");
            Console.ReadLine();
        }

        // -------------------------------------------------------------

        private List<LogEntry> LoadLogs()
        {
            var list = new List<LogEntry>();

            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(
                @"SELECT LogID, Username, Role, LoginTime, LogoutTime, ActionsNoted 
                  FROM Logs 
                  ORDER BY LogID DESC", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new LogEntry
                {
                    LogID = reader["LogID"] != DBNull.Value ? Convert.ToInt32(reader["LogID"]) : 0,
                    Username = reader["Username"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "",
                    LoginTime = reader["LoginTime"]?.ToString() ?? "",
                    LogoutTime = reader["LogoutTime"]?.ToString() ?? "",
                    ActionsNoted = reader["ActionsNoted"]?.ToString() ?? ""
                });
            }

            return list;
        }

        // -------------------------------------------------------------

        private class LogEntry
        {
            public int LogID { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
            public string LoginTime { get; set; }
            public string LogoutTime { get; set; }
            public string ActionsNoted { get; set; }
        }
    }
}
