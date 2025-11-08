using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class ViewMeetingsMenuItem: MenuItem
    {
        private int Role { get; set; }
        public ViewMeetingsMenuItem(int role)
        {
            Role = role;
        }
        public override string MenuText()
        {
            if (Role == 1) { return "View Meetings as Student"; }
            else if (Role == 2)
            {
                return "View Meetings as Supervisor";
            }
            else if (Role == 3)
            {
                return "View Meetings as Senior Tutor";
            }
            return "error";
        }
        public override void Select()
        {
            //TODO display user profile information
            if (Role == 1)
            {
                Console.WriteLine("Displaying Meetings for student ...");
            }
            else if (Role == 2)
            {
                Console.WriteLine("Displaying Meetings for supervisor ...");
            }
            else if (Role == 3)
            {
                Console.WriteLine("Displaying Meetings for senior tutor ...");
            }
        }
    }
}
