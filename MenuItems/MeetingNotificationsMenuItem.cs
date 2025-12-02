using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DDD_program.MenuItems
{
    internal class MeetingHUD
    {
        private string Username { get; }

        public MeetingHUD(string username)
        {
            Username = username;
        }

        public void DrawHUD()
        {
            Console.WriteLine("=== MEETINGS OVERVIEW ===\n");

            var allMeetings = LoadMeetings(Username);

            var pending = new List<Meeting>();
            var upcoming = new List<Meeting>();

            foreach (var m in allMeetings)
            {
                if (!m.Accepted)
                {
                    pending.Add(m);
                }
                else
                {
                    if (DateTime.TryParse(m.MeetingDate, out DateTime date))
                    {
                        if (date > DateTime.Now)
                            upcoming.Add(m);
                    }
                }
            }

            PrintSection("NEW / PENDING MEETINGS", pending);
            PrintSection("UPCOMING MEETINGS", upcoming);
            //PrintSection("PAST MEETINGS", past);
        }

        private void PrintSection(string title, List<Meeting> list)
        {
            Console.WriteLine($"--- {title} ---");

            if (list.Count == 0)
            {
                Console.WriteLine("   None\n");
                return;
            }

            foreach (var m in list)
            {
                Console.WriteLine($"• With: {GetOtherParty(m)}");
                Console.WriteLine($"  Date: {m.MeetingDate}");
                Console.WriteLine($"  Type: {m.MeetingType}");
                Console.WriteLine($"  Details: {m.Details}");
                Console.WriteLine($"  From Student: {(m.FromStudent ? "Yes" : "No")}");
                Console.WriteLine();
            }
        }

        private List<Meeting> LoadMeetings(string username)
        {
            var meetings = new List<Meeting>();

            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var cmd = new SQLiteCommand(
                @"SELECT MeetingID, Student, Supervisor, Role,
                         MeetingDate, Details, MeetingType, 
                         FromStudent, Accepted
                  FROM Meetings
                  WHERE Student=@u OR Supervisor=@u", conn);

            cmd.Parameters.AddWithValue("@u", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                meetings.Add(new Meeting
                {
                    Id = Convert.ToInt32(reader["MeetingID"]),
                    Student = reader["Student"].ToString(),
                    Supervisor = reader["Supervisor"].ToString(),
                    Role = reader["Role"].ToString(),
                    MeetingDate = reader["MeetingDate"].ToString(),
                    Details = reader["Details"].ToString(),
                    MeetingType = reader["MeetingType"]?.ToString() ?? "N/A",
                    FromStudent = Convert.ToBoolean(reader["FromStudent"]),
                    Accepted = Convert.ToBoolean(reader["Accepted"])
                });
            }

            return meetings;
        }

        private string GetOtherParty(Meeting m)
        {
            return m.Student == Username ? m.Supervisor : m.Student;
        }

        private class Meeting
        {
            public int Id { get; set; }
            public string Student { get; set; }
            public string Supervisor { get; set; }
            public string Role { get; set; }
            public string MeetingDate { get; set; }
            public string Details { get; set; }
            public string MeetingType { get; set; }
            public bool FromStudent { get; set; }
            public bool Accepted { get; set; }
        }
    }
}
