using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal class ExitMenuItem: MenuItem
    {
        // The parent in the menu hierarchy (not the base class) menu that will be exited if this item is selected
        private ConsoleMenu _menu;
        public ExitMenuItem(ConsoleMenu parentItem) => _menu = parentItem;

        //exit menu item text
        public override string MenuText()
        {
            return "Exit";
        }

        //leaving the menu
        public override void Select()
        {
            _menu.IsActive = false;
        }
    }
}
