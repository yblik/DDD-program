using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class MeetingHistoryMenuItem: MenuItem
    {
        private int Role { get; set; }
        public MeetingHistoryMenuItem(int role)
        {
            Role = role;
        }
        public override string MenuText()
        {
            if (Role == 1) { return "Meeting history as Student"; }
            else if (Role == 2)
            {
                return "Meeting history as Supervisor";
            }
            else if (Role == 3)
            {
                return "Meeting history as Senior Tutor";
            }
            return "error";
        }
        public override void Select()
        {
            SystemLogger.LogAction("Viewed meetings history menu");
            //TODO display user profile information
            if (Role == 1)
            {
                Console.WriteLine("Displaying Meeting history student ...");
            }
            else if (Role == 2)
            {
                Console.WriteLine("Displaying Meeting history supervisor ...");
            }
            else if (Role == 3)
            {
                Console.WriteLine("Displaying Meeting history senior tutor ...");
            }
        }
    }
}
