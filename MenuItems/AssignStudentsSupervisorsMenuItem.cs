using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class AssignStudentsSupervisorsMenuItem : MenuItem
    {
        public override string MenuText()
        {
            return "Assign Students to Supervisors";
        }

        public override void Select()
        {
            Console.WriteLine("\n=== ASSIGN STUDENTS TO SUPERVISORS ===");

            try
            {
                bool continueAssigning = true;

                while (continueAssigning)
                {
                    // Get all supervisors
                    var supervisors = GetAvailableUsers("supervisor");
                    if (supervisors.Count == 0)
                    {
                        Console.WriteLine("No supervisors available in the system.");
                        Console.WriteLine("\nPress ENTER to continue...");
                        Console.ReadLine();
                        return;
                    }

                    // Add exit option to supervisor list
                    var supervisorOptions = new List<string>();
                    foreach (var supervisor in supervisors)
                    {
                        string displayName = GetUserName(supervisor);
                        supervisorOptions.Add($"{displayName} ({supervisor})");
                    }
                    supervisorOptions.Add("Exit Assignment Menu");

                    // Select supervisor
                    int supervisorChoice = ConsoleHelper.GetSelectionFromMenu(supervisorOptions, "Select Supervisor:");

                    // Check if user selected exit
                    if (supervisorChoice == supervisorOptions.Count - 1)
                    {
                        Console.WriteLine("Exiting assignment menu.");
                        return;
                    }

                    string selectedSupervisor = supervisors[supervisorChoice];
                    string selectedSupervisorName = GetUserName(selectedSupervisor);

                    // Get available students (students not already assigned to this supervisor)
                    var availableStudents = GetAvailableStudentsForSupervisor(selectedSupervisor);
                    if (availableStudents.Count == 0)
                    {
                        Console.WriteLine($"No available students to assign to {selectedSupervisorName}.");
                        Console.WriteLine("\nPress ENTER to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    // Add exit option to student list
                    var studentOptions = new List<string>();
                    foreach (var student in availableStudents)
                    {
                        string displayName = GetUserName(student);
                        studentOptions.Add($"{displayName} ({student})");
                    }
                    studentOptions.Add("Back to Supervisor Selection");

                    // Select student
                    int studentChoice = ConsoleHelper.GetSelectionFromMenu(studentOptions, $"Select Student to assign to {selectedSupervisorName}:");

                    // Check if user selected back
                    if (studentChoice == studentOptions.Count - 1)
                    {
                        Console.WriteLine("Returning to supervisor selection...");
                        continue;
                    }

                    string selectedStudent = availableStudents[studentChoice];
                    string selectedStudentName = GetUserName(selectedStudent);

                    // Confirm assignment
                    string[] confirmOptions =
                    {
                        $"Yes, assign {selectedStudentName} to {selectedSupervisorName}",
                        "No, cancel assignment"
                    };

                    int confirmChoice = ConsoleHelper.GetSelectionFromMenu(confirmOptions,
                        $"Assign {selectedStudentName} to {selectedSupervisorName}?");

                    if (confirmChoice == 0)
                    {
                        AssignStudentToSupervisor(selectedStudent, selectedSupervisor);
                        Console.WriteLine($"Successfully assigned {selectedStudentName} to {selectedSupervisorName}!");
                    }
                    else
                    {
                        Console.WriteLine("Assignment cancelled.");
                    }

                    // Ask if user wants to assign more students
                    string[] continueOptions =
                    {
                        "Assign another student to supervisor",
                        "Exit assignment menu"
                    };

                    int continueChoice = ConsoleHelper.GetSelectionFromMenu(continueOptions, "\nWhat would you like to do next?");
                    continueAssigning = (continueChoice == 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during assignment: {ex.Message}");
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }

        private List<string> GetAvailableUsers(string role)
        {
            var users = new List<string>();

            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();
                string sql = @"
            SELECT u.Username
            FROM Users u
            LEFT JOIN Profiles p ON u.Username = p.Username
            WHERE u.Role = @role
            AND (p.Assigned IS NULL OR p.Assigned = '')
        ";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@role", role);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            users.Add(reader["Username"].ToString());
                    }
                }
            }

            return users;
        }


        private List<string> GetAvailableStudentsForSupervisor(string supervisorUsername)
        {
            var students = new List<string>();

            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();
                string sql = @"
            SELECT u.Username
            FROM Users u
            LEFT JOIN Profiles p ON u.Username = p.Username
            WHERE u.Role = 'student'
            AND (p.Assigned IS NULL OR p.Assigned = '')
        ";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        students.Add(reader["Username"].ToString());
                }
            }

            return students;
        }


        private void AssignStudentToSupervisor(string studentUsername, string supervisorUsername)
        {
            using (var conn = SQLstorage.GetConnection())
            {
                conn.Open();

                // Ensure Assigned column exists
                string alterSql = @"
            ALTER TABLE Profiles 
            ADD COLUMN Assigned TEXT DEFAULT NULL;
        ";

                using (var cmd = new SQLiteCommand(alterSql, conn))
                {
                    try { cmd.ExecuteNonQuery(); } catch { /* Column already exists */ }
                }

                // 1. Assign supervisor to student
                string assignStudentSql = @"
            UPDATE Profiles
            SET Assigned = @supervisor
            WHERE Username = @student
        ";

                using (var cmd = new SQLiteCommand(assignStudentSql, conn))
                {
                    cmd.Parameters.AddWithValue("@supervisor", supervisorUsername);
                    cmd.Parameters.AddWithValue("@student", studentUsername);
                    cmd.ExecuteNonQuery();
                }

                // 2. Assign student to supervisor
                string assignSupervisorSql = @"
            UPDATE Profiles
            SET Assigned = @student
            WHERE Username = @supervisor
        ";

                using (var cmd = new SQLiteCommand(assignSupervisorSql, conn))
                {
                    cmd.Parameters.AddWithValue("@student", studentUsername);
                    cmd.Parameters.AddWithValue("@supervisor", supervisorUsername);
                    cmd.ExecuteNonQuery();
                }
            }
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
                    return result?.ToString() ?? username;
                }
            }
        }
    }
}