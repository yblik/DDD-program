using DDD_program.MenuLogic;
using System;
using System.Data.SQLite;

namespace DDD_program.MenuItems
{
    class ProfileMenuItem : MenuItem
    {
        public int Role { get; set; }
        public string Username { get; set; }

        public ProfileMenuItem(int role, string username)
        {
            Role = role;
            Username = username;
        }

        public override string MenuText()
        {
            return Role switch
            {
                1 => "View Student Profile",
                2 => "View Supervisor Profile",
                3 => "View Senior Tutor Profile",
                _ => "View Profile"
            };
        }

        public override void Select()
        {
            Console.WriteLine("\n=== PROFILE ===");

            try
            {
                var profileData = FetchProfileData();
                if (profileData == null)
                {
                    Console.WriteLine("Profile not found.");
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();
                    return;
                }

                DisplayProfileInfo(profileData);

                if (profileData.Role.ToLower() == "student" && Role == 1)
                {
                    ShowStudentEditMenu(profileData.Name, profileData.HiddenAilments, profileData.ShareHidden);
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile: {ex.Message}");
                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();
            }
        }

        private ProfileData FetchProfileData()
        {
            using var conn = SQLstorage.GetConnection();
            conn.Open();

            // Get user role
            string role;
            using (var cmd = new SQLiteCommand("SELECT Role FROM Users WHERE Username=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", Username);
                var result = cmd.ExecuteScalar();
                if (result == null) return null;
                role = result.ToString();
            }

            // Get profile info
            string name = "";
            int age = 0, year = 0;
            using (var cmd = new SQLiteCommand("SELECT Name, Age, Year FROM Profiles WHERE Username=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", Username);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    name = reader["Name"]?.ToString() ?? "";
                    age = reader["Age"] != DBNull.Value ? Convert.ToInt32(reader["Age"]) : 0;
                    year = reader["Year"] != DBNull.Value ? Convert.ToInt32(reader["Year"]) : 0;
                }
                else
                {
                    return null;
                }
            }

            // Get health info for students
            string feeling = "", ailments = "", hiddenAilments = "";
            bool shareHidden = false;

            if (role.ToLower() == "student")
            {
                using (var cmd = new SQLiteCommand(
                    "SELECT Feeling, Ailments, HiddenAilments FROM HealthSupport WHERE Username=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@u", Username);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        feeling = reader["Feeling"]?.ToString() ?? "";
                        ailments = reader["Ailments"]?.ToString() ?? "";
                        hiddenAilments = reader["HiddenAilments"]?.ToString() ?? "";
                    }
                }

                // Check if hidden ailments are shared
                using (var cmd = new SQLiteCommand(
                    "SELECT HiddenAilments FROM HealthSupport WHERE Username=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@u", Username);
                    var result = cmd.ExecuteScalar();
                    shareHidden = result != null && result != DBNull.Value && Convert.ToInt32(result) == 0;
                }
            }

            return new ProfileData
            {
                Role = role,
                Name = name,
                Age = age,
                Year = year,
                Feeling = feeling,
                Ailments = ailments,
                HiddenAilments = hiddenAilments,
                ShareHidden = shareHidden
            };
        }

        private void DisplayProfileInfo(ProfileData data)
        {
            Console.WriteLine($"\nUsername: {Username}");
            Console.WriteLine($"Name: {data.Name}");
            Console.WriteLine($"Age: {data.Age}");
            Console.WriteLine($"Role: {data.Role}");

            if (data.Role.ToLower() == "student")
            {
                Console.WriteLine($"Academic Year: {data.Year}");
                Console.WriteLine($"Feeling: {data.Feeling}");
                Console.WriteLine($"Ailments: {data.Ailments}");

                if (Role == 1) // Student viewing own profile
                {
                    Console.WriteLine($"Hidden Ailments (private): {data.HiddenAilments}");
                    Console.WriteLine($"Shared with Supervisors: {(data.ShareHidden ? "YES" : "NO")}");
                }
                else // Supervisor viewing student profile
                {
                    Console.WriteLine($"Hidden Disabilities: {(data.ShareHidden ? data.HiddenAilments : "[NOT SHARED]")}");
                }
            }
        }

        private void ShowStudentEditMenu(string name, string hiddenAilments, bool shareHidden)
        {
            string[] options =
            {
                "Edit Name",
                "Edit Hidden Disabilities",
                "Toggle Sharing of Hidden Disabilities",
                "Back"
            };

            int choice = ConsoleHelper.GetSelectionFromMenu(options, "\nProfile Actions:");

            switch (choice)
            {
                case 0:
                    EditName();
                    break;
                case 1:
                    EditHiddenAilmentsText();
                    break;
                case 2:
                    ToggleHiddenAilmentsSharing();
                    break;
                default:
                    return;
            }
        }

        private void EditName()
        {
            string newName = ConsoleHelper.GetInput("Enter new name: ");
            if (string.IsNullOrWhiteSpace(newName))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            try
            {
                using var conn = SQLstorage.GetConnection();
                conn.Open();
                using var cmd = new SQLiteCommand("UPDATE Profiles SET Name=@n WHERE Username=@u", conn);
                cmd.Parameters.AddWithValue("@n", newName.Trim());
                cmd.Parameters.AddWithValue("@u", Username);
                int rowsAffected = cmd.ExecuteNonQuery();

                Console.WriteLine(rowsAffected > 0 ? "Name updated successfully!" : "Failed to update name.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating name: {ex.Message}");
            }
        }

        private void EditHiddenAilmentsText()
        {
            string newAilments = ConsoleHelper.GetInput("Enter hidden disabilities (leave empty to clear): ");

            try
            {
                using var conn = SQLstorage.GetConnection();
                conn.Open();
                using var cmd = new SQLiteCommand("UPDATE HealthSupport SET HiddenAilments=@h WHERE Username=@u", conn);
                cmd.Parameters.AddWithValue("@h", newAilments?.Trim() ?? "");
                cmd.Parameters.AddWithValue("@u", Username);
                int rowsAffected = cmd.ExecuteNonQuery();

                Console.WriteLine(rowsAffected > 0 ? "Hidden disabilities updated!" : "Failed to update hidden disabilities.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating hidden disabilities: {ex.Message}");
            }
        }

        private void ToggleHiddenAilmentsSharing()
        {
            try
            {
                using var conn = SQLstorage.GetConnection();
                conn.Open();

                // Get current value
                bool currentValue;
                using (var cmd = new SQLiteCommand("SELECT HiddenAilments FROM HealthSupport WHERE Username=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@u", Username);
                    var result = cmd.ExecuteScalar();
                    currentValue = result != null && result != DBNull.Value && Convert.ToInt32(result) == 0;
                }

                // Toggle value (0 = shared, 1 = not shared)
                bool newValue = !currentValue;

                using (var cmd = new SQLiteCommand("UPDATE HealthSupport SET HiddenAilments=@h WHERE Username=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@h", newValue ? 0 : 1);
                    cmd.Parameters.AddWithValue("@u", Username);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Hidden disabilities sharing {(newValue ? "ENABLED" : "DISABLED")}!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to update sharing settings.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling sharing: {ex.Message}");
            }
        }

        private class ProfileData
        {
            public string Role { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public int Year { get; set; }
            public string Feeling { get; set; }
            public string Ailments { get; set; }
            public string HiddenAilments { get; set; }
            public bool ShareHidden { get; set; }
        }
    }
}