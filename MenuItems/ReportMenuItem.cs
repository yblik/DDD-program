using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DDD_program.MenuLogic;

namespace DDD_program.MenuItems
{
    internal class ReportMenuItem: MenuItem
    {
        private int Role { get; set; }
        public ReportMenuItem(int role)
        {
            Role = role;
        }
        public override string MenuText()
        {
            Console.WriteLine(Role);
            if (Role == 1) { return "Report supervisor"; }
            if (Role == 2) { return "Report student"; }
            if (Role == 3) { return "Report performance concern"; }
            else { return "error"; }
        }
        public override void Select()
        {
            //TODO display user profile information
            if (Role == 1)
            {
                Console.WriteLine("Generating Report ...");
            }
        }
    }
}
