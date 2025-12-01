using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class MeetingNotificationsMenuItem : MenuItem
    {
        private string Username { get; }

        public MeetingNotificationsMenuItem(string username)
        {
            Username = username;
        }

        public override string MenuText() => "View Meeting Notifications";

        public override void Select()
        {
            Console.Clear();
            Console.WriteLine("=== MEETING NOTIFICATIONS ===\n");

            var meetings = GetPendingMeetings(Username);

            if (meetings.Count == 0)
            {
                Console.WriteLine("You have no pending meetings.");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < meetings.Count; i++)
            {
                var m = meetings[i];
                Console.WriteLine($"{i + 1}. Meeting with {GetOtherParty(m)}");
                Console.WriteLine($"   Date/Time: {m.MeetingDate}");
                Console.WriteLine($"   Details: {m.Details}");
                Console.WriteLine($"   From Student: {(m.FromStudent ? "Yes" : "No")}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPress ENTER to return or select a meeting to accept (feature coming soon).");
            Console.ReadLine();
        }

        private List<Meeting> GetPendingMeetings(string username)
        {
            var meetings = new List<Meeting>();

            using var conn = SQLstorage.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(
                @"SELECT Id, Student, Supervisor, MeetingDate, Details, FromStudent 
                  FROM Meetings 
                  WHERE (Student=@u OR Supervisor=@u) AND Accepted=0", conn);
            cmd.Parameters.AddWithValue("@u", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                meetings.Add(new Meeting
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Student = reader["Student"].ToString(),
                    Supervisor = reader["Supervisor"].ToString(),
                    MeetingDate = reader["MeetingDate"].ToString(),
                    Details = reader["Details"].ToString(),
                    FromStudent = reader["FromStudent"] != DBNull.Value && Convert.ToBoolean(reader["FromStudent"])
                });
            }

            return meetings;
        }

        private string GetOtherParty(Meeting meeting)
        {
            return meeting.Student == Username ? meeting.Supervisor : meeting.Student;
        }

        private class Meeting
        {
            public int Id { get; set; }
            public string Student { get; set; }
            public string Supervisor { get; set; }
            public string MeetingDate { get; set; }
            public string Details { get; set; }
            public bool FromStudent { get; set; }
        }
    }
}
