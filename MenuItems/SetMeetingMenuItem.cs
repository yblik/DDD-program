using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class SetMeetingMenuItem : MenuItem
    {
        private int Role { get; }
        private string Username { get; }
        private string UserRoleName { get; }

        public SetMeetingMenuItem(int role, string username)
        {
            Role = role;
            Username = username;
            UserRoleName = GetRoleName(role);
        }

        public override string MenuText() => "Set Meeting";

        public override void Select()
        {
            Console.Clear();
            Console.WriteLine("=== SET MEETING ===\n");

            try
            {
                SetMeeting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting meeting: {ex.Message}");
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }

        private void SetMeeting()
        {
            string studentUsername = null;
            string supervisorUsername = null;

            if (Role == 1) // Student
            {
                studentUsername = Username;
                supervisorUsername = GetAssignedSupervisor(studentUsername);

                if (string.IsNullOrEmpty(supervisorUsername))
                {
                    Console.WriteLine("No assigned supervisor found. Meeting creation cancelled.");
                    return;
                }
            }
            else if (Role == 2) // Supervisor
            {
                supervisorUsername = Username;
                var assignedStudents = GetAssignedStudents(supervisorUsername);
                if (assignedStudents.Count == 0)
                {
                    Console.WriteLine("No students assigned to you.");
                    return;
                }

                // Supervisor selects student
                studentUsername = SelectUserFromList(assignedStudents, "Select student for the meeting:");
            }
            else if (Role == 3) // Senior Tutor
            {
                // Senior tutor can select both supervisor and student
                var supervisors = GetAvailableUsers("supervisor");
                if (supervisors.Count == 0)
                {
                    Console.WriteLine("No supervisors available.");
                    return;
                }
                supervisorUsername = SelectUserFromList(supervisors, "Select supervisor for the meeting:");

                var students = GetAvailableUsers("student");
                if (students.Count == 0)
                {
                    Console.WriteLine("No students available.");
                    return;
                }
                studentUsername = SelectUserFromList(students, "Select student for the meeting:");
            }

            // Meeting details
            string date = ConsoleHelper.GetInput("Enter meeting date (YYYY-MM-DD): ");
            if (string.IsNullOrEmpty(date)) { Console.WriteLine("Meeting date is required."); return; }

            string time = ConsoleHelper.GetInput("Enter meeting time (HH:MM): ");
            if (string.IsNullOrEmpty(time)) { Console.WriteLine("Meeting time is required."); return; }

            string details = ConsoleHelper.GetInput("Enter meeting details: ");
            if (string.IsNullOrEmpty(details)) details = "No details provided";

            string fullDateTime = $"{date} {time}";
            bool fromStudent = Role == 1;

            // Confirm creation
            string confirmationMessage =
                $"\nCreate meeting?\n" +
                $"Student: {GetName(studentUsername)}\n" +
                $"Supervisor: {GetName(supervisorUsername)}\n" +
                $"Date/Time: {fullDateTime}\n" +
                $"Details: {details}\n" +
                $"Initiator: {(fromStudent ? "Student" : UserRoleName)}";

            string[] confirmOptions = { "Yes, create meeting", "No, cancel" };
            int confirmChoice = ConsoleHelper.GetSelectionFromMenu(confirmOptions, confirmationMessage);

            if (confirmChoice == 0)
            {
                InsertMeeting(studentUsername, supervisorUsername, fullDateTime, details, fromStudent);
                Console.WriteLine("Meeting created successfully!");
            }
            else
            {
                Console.WriteLine("Meeting creation cancelled.");
            }
        }

        private string SelectUserFromList(List<string> usernames, string prompt)
        {
            var displayNames = new List<string>();
            foreach (var u in usernames)
                displayNames.Add($"{GetName(u)} ({u})");

            int choice = ConsoleHelper.GetSelectionFromMenu(displayNames, prompt);
            return usernames[choice];
        }

        private string GetAssignedSupervisor(string studentUsername)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Assigned FROM Profiles WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", studentUsername);
            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }

        private List<string> GetAssignedStudents(string supervisorUsername)
        {
            var list = new List<string>();
            using var conn = SQLstorage.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Assigned FROM Profiles WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", supervisorUsername);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                foreach (var student in result.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries))
                    list.Add(student.Trim());
            }
            return list;
        }

        private List<string> GetAvailableUsers(string role)
        {
            var users = new List<string>();
            using var conn = SQLstorage.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Username FROM Users WHERE Role=@role", conn);
            cmd.Parameters.AddWithValue("@role", role);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(reader["Username"].ToString());
            }
            return users;
        }

        private string GetName(string username)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Name FROM Profiles WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? username;
        }

        private void InsertMeeting(string student, string supervisor, string dateTime, string details, bool fromStudent)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();
            string sql = @"INSERT INTO Meetings (Student, Supervisor, MeetingDate, Details, FromStudent, Accepted)
                           VALUES (@student, @supervisor, @dateTime, @details, @fromStudent, @accepted)";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@student", student);
            cmd.Parameters.AddWithValue("@supervisor", supervisor);
            cmd.Parameters.AddWithValue("@dateTime", dateTime);
            cmd.Parameters.AddWithValue("@details", details);
            cmd.Parameters.AddWithValue("@fromStudent", fromStudent);
            cmd.Parameters.AddWithValue("@accepted", false);
            cmd.ExecuteNonQuery();
        }

        private string GetRoleName(int role) => role switch
        {
            1 => "Student",
            2 => "Supervisor",
            3 => "SeniorTutor",
            _ => "User"
        };
    }
}
