using DDD_program.MenuLogic;
using DDD_program.Menus;
using DDD_program.Menus;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal class RoleLog //used to log role and kick start menus
    {
        public enum Role { student, Supervisor, SeniorTutor }
        public string Name;

        public int AgeOrStaffID; // for customer age or staff ID

        public Role SelectedProfile;



        public int RoleID; //bc csharp is retarded at times

        private bool selectedManager;
        private bool GOLD;
        public bool NONMEMBER;

        //for after login so roles are hardset
        public void SetProfile(int Selection)
        {
            if (Selection == 1)
            {
                SelectedProfile = Role.student;
                RoleID = 1;

                //TODO method to fetch username and find password
            }
            else if (Selection == 2)
            {
                SelectedProfile = Role.Supervisor;
                RoleID = 2;
                selectedManager = false; 
            }
            else
            {
                SelectedProfile = Role.SeniorTutor;
                RoleID = 3;
                selectedManager = true;
            }
        }
        public void Login()
        {
            while (true) // loop until successful login
            {
                Console.WriteLine("Login or create new account by inputting: ''new'' ");
                //Ask for username
                string username = ConsoleHelper.GetInput("Enter username:");

                if (username != "new")
                {
                    //Fetch the stored password and role from SQLite
                    string storedPassword = null;
                    string storedRole = null;

                    using (var connection = SQLstorage.GetConnection())
                    {
                        connection.Open();
                        string query = "SELECT Password, Role FROM Users WHERE Username = @username";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@username", username); 
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    storedPassword = reader.GetString(0); // Password column
                                    storedRole = reader.GetString(1);     // Role column
                                }
                            }
                        }
                    }

                    // Step 3: If username not found, show error and loop
                    if (storedPassword == null)
                    {
                        Console.WriteLine($"Username '{username}' not found. Try again or create new account.");
                        continue; // loop back to ask for username
                    }

                    // Step 4: Prompt for password
                    string inputPassword = ConsoleHelper.GetInput("Enter password:");

                    // Step 5: Check password
                    if (inputPassword != storedPassword)
                    {
                        Console.WriteLine("Incorrect password. Try again.");
                        continue; // loop back to ask for username again
                    }

                    // Step 6: Successful login, set profile/role
                    Name = username;

                    switch (storedRole.ToLower())
                    {
                        case "student":
                            SetProfile(1); // RoleID for student
                            SystemLogger.LogLogin(username, "student");
                            break;
                        case "supervisor":
                            SetProfile(2); // RoleID for supervisor
                            SystemLogger.LogLogin(username, "supervisor");
                            break;
                        case "senior tutor":
                            SetProfile(3); // RoleID for senior tutor
                            SystemLogger.LogLogin(username, "senior tutor");
                            break;
                        default:
                            Console.WriteLine("Unknown role. Contact admin.");
                            return;
                    }
                }

                else
                {
                    CreateAccount CA = new CreateAccount();
                    CA.CreateNewUser();
                    continue; // important 
                }
                // Step 7: Launch main menu

                MainMenu MM = new MainMenu(RoleID, Name);
                MM.Select();

                break; // exit login loop after successful login
            }
        }


    }
}
/*SQLstorage.Initialize();
SQLmanager.AddUserWithDetails("bob123", "password", "Student", "Bob Smith", 20, 2, "Happy", "None", "None");
SQLmanager.AddLog("bob123", "Student", "Logged in");
 * 
 * 
 */
