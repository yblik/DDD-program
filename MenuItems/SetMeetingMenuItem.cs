using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class SetMeetingMenuItem: MenuItem
    {
        private int Role { get; set; }
        public SetMeetingMenuItem(int role)
        {
            Role = role;
        }
        public override string MenuText()
        {
            if (Role == 1) { return "Set meeting as Student"; }
            else if (Role == 2)
            {
                return "Set meeting as Supervisor";
            }
            else if (Role == 3)
            {
                return "Set meeting as Senior Tutor";
            }
            return "View Profile";
        }
        public override void Select()
        {
            //TODO display user profile information
            if (Role == 1)
            {
                Console.WriteLine("Displaying student meeting setup...");
            }
            else if (Role == 2)
            {
                Console.WriteLine("Displaying supervisor meeting setup...");
            }
            else if (Role == 3)
            {
                Console.WriteLine("Displaying senior tutor meeting setup...");
            }
        }
    }
}
