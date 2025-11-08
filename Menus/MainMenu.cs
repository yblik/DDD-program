using DDD_program.MenuItems;
using DDD_program.MenuLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program.Menus
{
    class MainMenu: ConsoleMenu
    {
        public int Role { get; set; }

        //plan is to have a calender ascii art table thing to be drawn out when logged in

        public MainMenu(int role)
        {
            Role = role;
            //Schedule = new Scheduler();
            //concsv = new ConcessionCSV();
            //cuscsv = new CustomerCSV();
        }
        /// <summary>
        /// menus and shi classes to be used
        /// </summary>
        //private ProfileMenuItem PMI;
        //private ConcessionCSV concsv;
        //public CustomerCSV cuscsv;

        public void PopUpFeeling()
        {
            //are you feeling poppy today?
        }

        public override void CreateMenu()
        {
            _menuItems.Clear();
            //TODO add menu items based on role

            //logged in as student
            //Set meetings
            _menuItems.Add(new SetMeetingMenuItem(Role));

            //View profile
            _menuItems.Add(new ProfileMenuItem(Role));
            //meeting history
            _menuItems.Add(new MeetingHistoryMenuItem(Role));
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Upcoming meetings:");
            Console.WriteLine("10.11.26 - [confirmed]");
            Console.WriteLine("26.11.26 - [need confirmation]");
            //--upcoming meetings section
            //--view meeting details
            _menuItems.Add(new ViewMeetingsMenuItem(Role));
            Console.WriteLine("---------------------------------------------------");

            //report faculty 
            _menuItems.Add(new ReportMenuItem(Role));

            //_menuItems.Add(new SelectFilmCustomerMenuItem(Schedule, ID, this)); //this for exitting
            //_menuItems.Add(new SelectConcessCustomerMenuItem(this, concsv, ID)); //this for exitting
            //_menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return $"Choose action";
        }
    }
}
