using DDD_program.MenuLogic;
using System;

namespace DDD_program
{
    internal class CreateAccount
    {
        private const string AdminPassword = "SuperSecret123"; // temporary

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
            }
            while (true);

            // Step 2: Password
            string password = ConsoleHelper.GetInput("Enter password:");

            // Step 3: Role selection
            string[] roles = { "Student", "Supervisor", "Senior Tutor" };
            int roleIndex = ConsoleHelper.GetSelectionFromMenu(roles, "Select role:");
            string role = roles[roleIndex];

            // === Admin Lock for Supervisor or Senior Tutor ===
            if (role != "Student")
            {
                string adminTry = ConsoleHelper.GetInput("Enter admin password to create staff accounts:");

                if (adminTry != AdminPassword)
                {
                    Console.WriteLine("Incorrect admin password. Defaulting to Student role.");
                    role = "Student";
                }
            }

            // Step 4: Profile info
            string name = ConsoleHelper.GetInput("Enter full name:");


            int age = ConsoleHelper.GetIntegerInRange(18, 90, "Enter age between 18 and 90");

            // Student-only academic year adjustment
            int year = 0;
            if (role == "Student")
            {
                string[] yearOptions = { "2023", "2024", "2025" };
                year = roleIndex == 1 ?
                    ConsoleHelper.GetSelectionFromMenu(yearOptions, "Enter years at establishment") :
                    ConsoleHelper.GetIntegerInRange(1, 60, "Enter years at establishment");
            }

            // Step 5: Health & Support info (students only)
            string feeling = "";
            string ailments = "";
            string hiddenAilments = "";

            if (role == "Student")
            {
                hiddenAilments = ConsoleHelper.GetInput(
                    "Enter any ailments (optional — these are hidden unless shared):"
                );
            }

            // Step 6: Add user to all tables using SQLmanager
            SQLmanager.AddUserWithDetails(
                username,
                password,
                role,
                name,
                year,
                age,
                feeling,
                ailments,
                hiddenAilments
            );

            Console.WriteLine($"User '{username}' successfully created!");
            Console.WriteLine("Returning to login...\n");

            return; //exit method to login
        }
    }
}
