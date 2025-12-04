using DDD_program.MenuLogic;
using System;
using System.Data.SQLite;

namespace DDD_program.MenuItems
{
    internal class SetFeelingMenuItem : MenuItem
    {
        private string Username { get; }

        public SetFeelingMenuItem(string username)
        {
            Username = username;
        }

        public override string MenuText() => "Set Feeling";

        public override void Select()
        {
            SystemLogger.LogAction("Student went on feelings menu");
            Console.Clear();
            Console.WriteLine("=== SET FEELING ===\n");

            try
            {
                SetFeeling();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting feeling: {ex.Message}");
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }

        private void SetFeeling()
        {
            string currentFeeling = GetCurrentFeeling(Username);
            Console.WriteLine($"Your current feeling: {currentFeeling}\n");

            string[] feelings =
            { "Happy", "Excited", "Neutral", "Sad", "Anxious", "Stressed", "Depressed","Overwhelmed","Angry", "Zesty", "Freaky" };

            int feelingIndex = ConsoleHelper.GetSelectionFromMenu(feelings, "Select your current feeling:");

            string selectedFeeling = feelings[feelingIndex];
            SaveFeeling(Username, selectedFeeling);
            Console.WriteLine($"\nFeeling updated to: {selectedFeeling}");

            // If index > 3 (Sad or worse), ask if a meeting should be created
            if (feelingIndex > 3)
            {
                string[] options = { "Yes, schedule a meeting", "No, not now" };
                int choice = ConsoleHelper.GetSelectionFromMenu(options, "Your feeling may require a meeting. Create a meeting now?");
                if (choice == 0)
                {
                    var setMeeting = new SetMeetingMenuItem(1, Username); // Role 1 = student
                    setMeeting.Select();
                }
            }
        }


        private string GetCurrentFeeling(string username)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Feeling FROM HealthSupport WHERE Username=@u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "Neutral";
        }

        private void SaveFeeling(string username, string feeling)
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM HealthSupport WHERE Username=@u", conn);
            checkCmd.Parameters.AddWithValue("@u", username);
            long count = (long)checkCmd.ExecuteScalar();

            string sql = count == 0
                ? "INSERT INTO HealthSupport (Username, Feeling) VALUES (@u, @f)"
                : "UPDATE HealthSupport SET Feeling=@f WHERE Username=@u";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@f", feeling);
            cmd.ExecuteNonQuery();
        }
    }
}
