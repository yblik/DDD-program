using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class SetMeetingMenuItem : MenuItem
    {
        private int Role { get; set; }
        private string Username { get; set; }
        private string UserRoleName { get; set; }

        public SetMeetingMenuItem(int role, string username)
        {
            Role = role;
            Username = username;
            UserRoleName = GetRoleName(role);
        }

        public override string MenuText()
        {
            return "Set Meeting";
        }

        public override void Select()
        {
            Console.WriteLine("\n=== SET MEETING ===");

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
            // Get student name
            string studentName = GetStudentName();
            if (string.IsNullOrEmpty(studentName))
            {
                Console.WriteLine("No student selected. Meeting creation cancelled.");
                return;
            }

            // Get supervisor name
            string supervisorName = GetSupervisorName();
            if (string.IsNullOrEmpty(supervisorName))
            {
                Console.WriteLine("No supervisor selected. Meeting creation cancelled.");
                return;
            }

            // Get meeting details
            string meetingDate = GetMeetingDate();
            if (string.IsNullOrEmpty(meetingDate))
            {
                Console.WriteLine("Meeting date is required.");
                return;
            }

            string meetingTime = GetMeetingTime();
            if (string.IsNullOrEmpty(meetingTime))
            {
                Console.WriteLine("Meeting time is required.");
                return;
            }

            string details = ConsoleHelper.GetInput("Enter meeting details: ");
            if (string.IsNullOrEmpty(details))
            {
                details = "No details provided";
            }

            // Combine date and time
            string fullMeetingDateTime = $"{meetingDate} {meetingTime}";

            // Determine if this is from a student
            bool fromStudent = (Role == 1) ? true: false; // Role 1 is student

            // Confirm meeting creation
            string initiator = fromStudent ? "Student" : UserRoleName;
            string confirmationMessage =
                $"\nCreate meeting?\n" +
                $"Student: {studentName}\n" +
                $"Supervisor: {supervisorName}\n" +
                $"Date/Time: {fullMeetingDateTime}\n" +
                $"Details: {details}\n" +
                $"Initiator: {initiator}\n\n" +
                $"Proceed with creation?";

            // Use selection menu for confirmation
            string[] confirmOptions = { "Yes, create meeting", "No, cancel" };
            int confirmChoice = ConsoleHelper.GetSelectionFromMenu(confirmOptions, confirmationMessage);

            if (confirmChoice == 0) // Yes
            {
                CreateMeeting(studentName, UserRoleName, fullMeetingDateTime, details, supervisorName, fromStudent);
                Console.WriteLine("Meeting created successfully!");
            }
            else
            {
                Console.WriteLine("Meeting creation cancelled.");
            }
        }

        private string GetStudentName()
        {
            if (Role == 1) // If current user is a student, use their own name
            {
                return GetUserName(Username);
            }

            var students = GetAvailableUsers("student");
            if (students.Count == 0)
            {
                Console.WriteLine("No students available.");
                return null;
            }

            // Create display names for the menu
            var studentDisplayNames = new List<string>();
            foreach (var student in students)
            {
                string displayName = GetUserName(student);
                studentDisplayNames.Add($"{displayName} ({student})");
            }

            int choice = ConsoleHelper.GetSelectionFromMenu(studentDisplayNames, "\nSelect student:");
            return students[choice];
        }

        private string GetSupervisorName()
        {
            if (Role == 2) // If current user is a supervisor, use their own name
            {
                return Username;
            }

            var supervisors = GetAvailableUsers("supervisor");
            if (supervisors.Count == 0)
            {
                Console.WriteLine("No supervisors available.");
                return null;
            }

            // Create display names for the menu
            var supervisorDisplayNames = new List<string>();
            foreach (var supervisor in supervisors)
            {
                string displayName = GetUserName(supervisor);
                supervisorDisplayNames.Add($"{displayName} ({supervisor})");
            }

            int choice = ConsoleHelper.GetSelectionFromMenu(supervisorDisplayNames, "\nSelect supervisor:");
            return supervisors[choice];
        }

        private string GetMeetingDate()
        {
            // Use GetInput for date (could be enhanced with validation)
            return ConsoleHelper.GetInput("Enter meeting date (YYYY-MM-DD): ");
        }

        private string GetMeetingTime()
        {
            // Use GetInput for time (could be enhanced with validation)
            return ConsoleHelper.GetInput("Enter meeting time (HH:MM): ");
        }

        private string GetUserName(string username)
        {
            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Name FROM Profiles WHERE Username = @username";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? username; // Return name if found, otherwise return username
                }
            }
        }

        private List<string> GetAvailableUsers(string role)
        {
            var users = new List<string>();

            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Username FROM Users WHERE Role = @role";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@role", role);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(reader["Username"].ToString());
                        }
                    }
                }
            }

            return users;
        }

        private void CreateMeeting(string student, string role, string meetingDateTime, string details, string supervisor, bool fromStudent)
        {
            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO Meetings (Student, Role, MeetingDate, Details, Supervisor, FromStudent, Accepted) 
                              VALUES (@student, @role, @meetingDate, @details, @supervisor, @fromStudent, @accepted)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@student", student);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@meetingDate", meetingDateTime);
                    cmd.Parameters.AddWithValue("@details", details);
                    cmd.Parameters.AddWithValue("@supervisor", supervisor);
                    cmd.Parameters.AddWithValue("@fromStudent", fromStudent);
                    cmd.Parameters.AddWithValue("@accepted", false); // Default to not accepted

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GetRoleName(int role)
        {
            return role switch
            {
                1 => "Student",
                2 => "Supervisor",
                3 => "SeniorTutor",
                _ => "User"
            };
        }
    }
}