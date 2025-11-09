using DDD_program.MenuLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal class CreateAccount
    {
        public void CreateNewUser()
        {
            Console.WriteLine("=== Create New User ===");

            // Step 1: Username
            string username;
            do
            {
                username = ConsoleHelper.GetInput("Enter username:");
                if (!SQLmanager.RecordExists("Users", "Username", username))
                    break;

                Console.WriteLine($"Username '{username}' already exists. Try another one.");
            } while (true);

            // Step 2: Password
            string password = ConsoleHelper.GetInput("Enter password:");

            // Step 3: Role selection
            string[] roles = { "Student", "Supervisor", "Senior Tutor" };
            int roleIndex = ConsoleHelper.GetSelectionFromMenu(roles, "Select role:");
            string role = roles[roleIndex]; // Convert index to role string- helps later with using index instead of word

            // Step 4: Profile info
            string name = ConsoleHelper.GetInput("Enter full name:");
            int year = 0;

            if (role == "Student")
            {
                string msg = "Enter academic year(e.g., Year 1-4)";
                year = ConsoleHelper.GetIntegerInRange(1, 4, msg);
            }

            // Step 5: Health & Support info (optional, only for students)
            string feeling = "";
            string ailments = "";
            string hiddenAilments = "";
            if (role == "Student")
            {
                //ailments = ConsoleHelper.GetInput("Enter any known ailments (comma separated) or 'None':");
                hiddenAilments = ConsoleHelper.GetInput("Enter any ailments (optional). (These are automatically hidden but can be shared in student profile):");
            }

            // Step 6: Add user to all tables using SQLmanager
            SQLmanager.AddUserWithDetails(
                username,
                password,
                role,
                name,
                year,
                feeling, //just fill table
                ailments, //just fill table
                hiddenAilments
            );

            Console.WriteLine($"User '{username}' successfully created!");
        }
    }
}
