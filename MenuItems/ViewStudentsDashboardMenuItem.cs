using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class ViewStudentsDashboardMenuItem : MenuItem
    {
        private string SupervisorUsername;

        public ViewStudentsDashboardMenuItem(string supervisorUsername)
        {
            SupervisorUsername = supervisorUsername;
        }

        public override string MenuText()
        {
            return "View Students Dashboard";
        }

        public override void Select()
        {
            Console.Clear();
            Console.WriteLine("=== STUDENTS DASHBOARD ===\n");

            List<string> studentUsernames = GetAssignedStudents(SupervisorUsername);

            if (studentUsernames.Count == 0)
            {
                Console.WriteLine("No students assigned to you.");
                Console.ReadLine();
                return;
            }

            var students = new List<StudentInfo>();
            foreach (var username in studentUsernames)
            {
                var info = FetchStudentInfo(username);
                if (info != null)
                    students.Add(info);
            }

            // Display students
            for (int i = 0; i < students.Count; i++)
            {
                var s = students[i];
                Console.WriteLine($"{i + 1}. {s.Username}");
                Console.WriteLine($"   Feeling: {s.Feeling}");

                if (s.ShareHidden)
                {
                    string ailmentsToShow = string.IsNullOrWhiteSpace(s.Ailments) ? "None reported" : s.Ailments;
                    Console.WriteLine($"   Ailments: {ailmentsToShow}");
                }
                else
                {
                    Console.WriteLine($"   Ailments: [NOT SHARED]");
                }

                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("Press ENTER to return to the main menu...");
            Console.ReadLine();
        }

        private List<string> GetAssignedStudents(string supervisorUsername)
        {
            var list = new List<string>();
            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();
                using var cmd = new SQLiteCommand("SELECT Assigned FROM Profiles WHERE Username=@u", conn);
                cmd.Parameters.AddWithValue("@u", supervisorUsername);
                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string csv = result.ToString();
                    foreach (var student in csv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        list.Add(student.Trim());
                }
            }
            return list;
        }

        private StudentInfo FetchStudentInfo(string username)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            var info = new StudentInfo { Username = username };

            // Get basic profile info
            using (var cmd = new SQLiteCommand("SELECT Age, Year FROM Profiles WHERE Username=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    info.Age = reader["Age"] != DBNull.Value ? Convert.ToInt32(reader["Age"]) : 0;
                    info.Year = reader["Year"] != DBNull.Value ? Convert.ToInt32(reader["Year"]) : 0;
                }
                else
                {
                    return null;
                }
            }

            // Get Feeling and Ailments
            using (var cmd = new SQLiteCommand("SELECT Feeling, Ailments, HiddenAilments FROM HealthSupport WHERE Username=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    info.Feeling = reader["Feeling"]?.ToString() ?? "";
                    info.Ailments = reader["Ailments"]?.ToString() ?? "";
                    int hiddenFlag = reader["HiddenAilments"] != DBNull.Value ? Convert.ToInt32(reader["HiddenAilments"]) : 1;
                    info.ShareHidden = hiddenFlag == 0; // 0 = shared
                }
                else
                {
                    info.Feeling = "";
                    info.Ailments = "";
                    info.ShareHidden = false;
                }
            }

            return info;
        }

        private class StudentInfo
        {
            public string Username { get; set; }
            public int Age { get; set; }
            public int Year { get; set; }
            public string Feeling { get; set; }
            public string Ailments { get; set; }
            public bool ShareHidden { get; set; } // true = shared (HiddenAilments == 0)
        }
    }
}
