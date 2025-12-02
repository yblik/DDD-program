using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class ReportMenuItem : MenuItem
    {
        private int Role;                  // 1 = Student, 2 = Supervisor, 3 = Senior Tutor
        private string CurrentUsername;    // Passed in from the menu system

        public ReportMenuItem(int role, string username)
        {
            Role = role;
            CurrentUsername = username;
        }

        public override string MenuText()
        {
            if (Role == 1) return "Report supervisor";
            if (Role == 2) return "Report student";
            if (Role == 3) return "Report performance concern";
            return "error";
        }

        public override void Select()
        {
            SystemLogger.LogAction("Opened report menu");

            Console.Clear();
            Console.WriteLine("=== REPORT MENU ===\n");

            string respondent = "";
            string reportType = "";
            string detail = "";

            switch (Role)
            {
                case 1: // Student -> Report assigned supervisor
                    reportType = "Supervisor Report";
                    respondent = GetAssignedSupervisor(CurrentUsername);

                    if (string.IsNullOrWhiteSpace(respondent))
                    {
                        Console.WriteLine("You have no assigned supervisor. Cannot create report.");
                        Console.ReadLine();
                        return;
                    }

                    Console.WriteLine($"You are reporting your supervisor: {respondent}");
                    break;

                case 2: // Supervisor -> Must choose from assigned students
                    reportType = "Student Report";
                    var students = GetAssignedStudents(CurrentUsername);

                    if (students.Count == 0)
                    {
                        Console.WriteLine("You have no assigned students to report.");
                        Console.ReadLine();
                        return;
                    }

                    Console.WriteLine("Select a student to report:\n");

                    for (int i = 0; i < students.Count; i++)
                        Console.WriteLine($"{i + 1}. {students[i]}");

                    Console.Write("\nSelection: ");
                    if (!int.TryParse(Console.ReadLine(), out int idx) ||
                        idx < 1 || idx > students.Count)
                    {
                        Console.WriteLine("Invalid selection.");
                        Console.ReadLine();
                        return;
                    }

                    respondent = students[idx - 1];
                    break;

                case 3: // Senior Tutor -> free reporting
                    reportType = "Performance Concern";

                    Console.Write("Enter username to report: ");
                    respondent = Console.ReadLine();
                    break;

                default:
                    Console.WriteLine("Invalid role.");
                    return;
            }

            Console.Write("\nEnter report details: ");
            detail = Console.ReadLine();

            SaveReport(CurrentUsername, respondent, Role, reportType, detail);

            Console.WriteLine("\nReport submitted successfully.");
            SystemLogger.LogAction($"Report submitted about {respondent} ({reportType})");
            Console.ReadLine();
        }

        // ------------------- HELPERS ---------------------

        private string GetAssignedSupervisor(string student)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand("SELECT Supervisor FROM Profiles WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", student);

            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }

        private List<string> GetAssignedStudents(string supervisor)
        {
            var list = new List<string>();

            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand("SELECT Assigned FROM Profiles WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", supervisor);

            var result = cmd.ExecuteScalar();
            if (result == null) return list;

            var csv = result.ToString();
            if (string.IsNullOrWhiteSpace(csv)) return list;

            foreach (var s in csv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                list.Add(s.Trim());

            return list;
        }

        private void SaveReport(string username, string respondent, int role, string reportType, string detail)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(@"
                INSERT INTO Reports (Username, Respondent, Role, ReportType, Detail)
                VALUES (@u, @r, @role, @t, @d)
            ", conn);

            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@r", respondent);
            cmd.Parameters.AddWithValue("@role", role.ToString());
            cmd.Parameters.AddWithValue("@t", reportType);
            cmd.Parameters.AddWithValue("@d", detail);

            cmd.ExecuteNonQuery();
        }
    }
}
