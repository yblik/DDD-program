using System;
using System.Collections.Generic;
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
            Console.Clear();
            Console.WriteLine("=== ASSIGN STUDENT TO SUPERVISOR ===\n");

            // 1. Get supervisors
            var supervisors = SQLmanager.GetUsernamesByRole("Supervisor");
            if (supervisors.Count == 0)
            {
                Console.WriteLine("No supervisors found.");
                Console.ReadLine();
                return;
            }

            // Show supervisors
            Console.WriteLine("Select Supervisor:");
            for (int i = 0; i < supervisors.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {supervisors[i]}");
            }
            Console.Write($"\nEnter choice (1-{supervisors.Count}): ");

            int superIndex;
            if (!int.TryParse(Console.ReadLine(), out superIndex) || superIndex < 1 || superIndex > supervisors.Count)
            {
                Console.WriteLine("Invalid selection.");
                Console.ReadLine();
                return;
            }

            string selectedSupervisor = supervisors[superIndex - 1];

            // 2. Get students
            var students = SQLmanager.GetUsernamesByRole("Student");
            if (students.Count == 0)
            {
                Console.WriteLine("No students found.");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine($"=== ASSIGN TO SUPERVISOR: {selectedSupervisor} ===\n");
            Console.WriteLine("Select Student:");

            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i]}");
            }
            Console.Write($"\nEnter choice (1-{students.Count}): ");

            int studentIndex;
            if (!int.TryParse(Console.ReadLine(), out studentIndex) || studentIndex < 1 || studentIndex > students.Count)
            {
                Console.WriteLine("Invalid selection.");
                Console.ReadLine();
                return;
            }

            string selectedStudent = students[studentIndex - 1];

            // 3. UPDATE THE FUCKING TABLE
            bool success = SQLmanager.AssignStudentToSupervisor(selectedStudent, selectedSupervisor);

            if (success)
            {
                Console.WriteLine($"\n✅ Assigned {selectedStudent} to {selectedSupervisor}");
            }
            else
            {
                Console.WriteLine($"\n❌ Failed to assign.");
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }
}