using DDD_program.MenuLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program.MenuItems
{
    class ProfileMenuItem: MenuItem //for all user profile related actions
    {
        public int Role { get; set; } //user role to determine what profile info to show
        public ProfileMenuItem(int role)
        {
            Role = role;
        }
        public override string MenuText()
        {
            if (Role == 1) { return "View Student Profile"; }
            else if (Role == 2) {
                return "View Supervisor Profile";
            }
            else if (Role == 3) {
                return "View Senior Tutor Profile";
            }
            return "View Profile";
        }
        public override void Select()
        {
            //TODO display user profile information
            if (Role == 1)
            {
                Console.WriteLine("Displaying student profile...");
            }
            else if (Role == 2)
            {
                Console.WriteLine("Displaying supervisor profile...");
            }
            else if (Role == 3)
            {
                Console.WriteLine("Displaying senior tutor profile...");
            }
        }
    }
}
