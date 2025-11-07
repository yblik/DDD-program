using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal class MainMenu: ConsoleMenu
    {
        public int Role { get; set; }

        //plan is to have a calender ascii art table thing to be drawn out when logged in

        MainMenu(int role)
        {
            Role = role;
            //Schedule = new Scheduler();
            //concsv = new ConcessionCSV();
            //cuscsv = new CustomerCSV();
        }
        /// <summary>
        /// menus and shi classes to be used
        /// </summary>
        //private Scheduler Schedule;
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
