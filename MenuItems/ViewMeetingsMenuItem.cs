using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class ViewMeetingsMenuItem : MenuItem
    {
        private int Role { get; set; } // 1 = Student, 2 = Supervisor, 3 = Senior Tutor
        private string Username { get; set; }

        public ViewMeetingsMenuItem(int role, string username)
        {
            Role = role;
            Username = username;
        }

        public override string MenuText()
        {
            return "View Meetings";
        }

        public override void Select()
        {
            SystemLogger.LogAction("Viewed Meetings menu");
            Console.Clear();
            Console.WriteLine("=== MEETINGS ===\n");

            List<Meeting> meetings = FetchMeetings();
            if (meetings.Count == 0)
            {
                Console.WriteLine("No meetings found.");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                var displayList = new List<string>();
                foreach (var m in meetings)
                {
                    string status = m.Accepted
                        ? "Accepted"
                        : (Role == 2 && !m.Accepted ? "Pending" : "Not Accepted");

                    displayList.Add(
                        $"{m.MeetingDate} | Student: {m.Student} | Supervisor: {m.Supervisor} | Role: {m.Role} | Status: {status}");
                }

                int selectedIndex = ConsoleHelper.GetSelectionFromMenu(displayList, "Select a meeting (ESC to exit):");
                if (selectedIndex < 0 || selectedIndex >= meetings.Count) break;

                var selectedMeeting = meetings[selectedIndex];
                ShowMeetingDetails(selectedMeeting);

                // Supervisors can accept, reschedule, cancel
                if (Role == 2)
                {
                    HandleSupervisorActions(selectedMeeting);
                }
                // Students can only reschedule/cancel their own meetings if not accepted
                else if (Role == 1)
                {
                    HandleStudentActions(selectedMeeting);
                }

                // Refresh meeting list after any changes
                meetings = FetchMeetings();
            }
        }

        private void ShowMeetingDetails(Meeting m)
        {
            Console.Clear();
            Console.WriteLine("=== MEETING DETAILS ===");
            Console.WriteLine($"Student: {m.Student}");
            Console.WriteLine($"Supervisor: {m.Supervisor}");
            Console.WriteLine($"Role: {m.Role}");
            Console.WriteLine($"Date/Time: {m.MeetingDate}");
            Console.WriteLine($"Details: {m.Details}");
            Console.WriteLine($"Accepted: {(m.Accepted ? "Yes" : "No")}");
            Console.WriteLine($"From Student: {(m.FromStudent ? "Yes" : "No")}");
            Console.WriteLine(new string('-', 30));
        }

        private void HandleSupervisorActions(Meeting m)
        {
            var options = new List<string>();
            if (!m.Accepted) options.Add("Accept Meeting");
            options.Add("Reschedule Meeting");
            options.Add("Cancel Meeting");
            options.Add("Back");

            int choice = ConsoleHelper.GetSelectionFromMenu(options.ToArray(), "Choose an action:");

            switch (options[choice])
            {
                case "Accept Meeting":
                    AcceptMeeting(m.Id);
                    Console.WriteLine("Meeting accepted!");
                    break;
                case "Reschedule Meeting":
                    RescheduleMeeting(m.Id);
                    break;
                case "Cancel Meeting":
                    CancelMeeting(m.Id);
                    break;
                default:
                    return;
            }

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private void HandleStudentActions(Meeting m)
        {
            if (m.Accepted)
            {
                Console.WriteLine("You cannot modify accepted meetings.");
                Console.ReadLine();
                return;
            }

            if (m.Student != Username)
            {
                Console.WriteLine("You can only modify your own meetings.");
                Console.ReadLine();
                return;
            }

            var options = new string[] { "Reschedule Meeting", "Cancel Meeting", "Back" };
            int choice = ConsoleHelper.GetSelectionFromMenu(options, "Choose an action:");

            switch (choice)
            {
                case 0: // Reschedule
                    RescheduleMeeting(m.Id);
                    break;
                case 1: // Cancel
                    CancelMeeting(m.Id);
                    break;
                default:
                    return;
            }

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private List<Meeting> FetchMeetings()
        {
            var list = new List<Meeting>();
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(
                "SELECT MeetingID, Student, Role, MeetingDate, Details, Supervisor, FromStudent, Accepted " +
                "FROM Meetings WHERE Student=@u OR Supervisor=@u", conn);
            cmd.Parameters.AddWithValue("@u", Username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Meeting
                {
                    Id = reader["MeetingID"] != DBNull.Value ? Convert.ToInt32(reader["MeetingID"]) : 0,
                    Student = reader["Student"]?.ToString() ?? "",
                    Supervisor = reader["Supervisor"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "General",
                    MeetingDate = reader["MeetingDate"]?.ToString() ?? "",
                    Details = reader["Details"]?.ToString() ?? "",
                    Accepted = reader["Accepted"] != DBNull.Value && Convert.ToBoolean(reader["Accepted"]),
                    FromStudent = reader["FromStudent"] != DBNull.Value && Convert.ToBoolean(reader["FromStudent"])
                });
            }

            return list;
        }

        private void AcceptMeeting(int meetingId)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand("UPDATE Meetings SET Accepted=@a WHERE MeetingID=@id", conn);
            cmd.Parameters.AddWithValue("@a", true);
            cmd.Parameters.AddWithValue("@id", meetingId);
            cmd.ExecuteNonQuery();
        }

        private void CancelMeeting(int meetingId)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand("DELETE FROM Meetings WHERE MeetingID=@id", conn);
            cmd.Parameters.AddWithValue("@id", meetingId);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Meeting cancelled.");
        }

        private void RescheduleMeeting(int meetingId)
        {
            string newDate = ConsoleHelper.GetInput("Enter new date (YYYY-MM-DD): ");
            string newTime = ConsoleHelper.GetInput("Enter new time (HH:MM): ");
            string newDateTime = $"{newDate} {newTime}";

            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand("UPDATE Meetings SET MeetingDate=@dt WHERE MeetingID=@id", conn);
            cmd.Parameters.AddWithValue("@dt", newDateTime);
            cmd.Parameters.AddWithValue("@id", meetingId);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Meeting rescheduled.");
        }

        private class Meeting
        {
            public int Id { get; set; }
            public string Student { get; set; }
            public string Supervisor { get; set; }
            public string Role { get; set; }
            public string MeetingDate { get; set; }
            public string Details { get; set; }
            public bool Accepted { get; set; }
            public bool FromStudent { get; set; }
        }
    }
}
