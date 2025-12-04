using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program.MenuLogic
{
    public abstract class ConsoleMenu: MenuItem
    {
        //list of menu items
        protected List<MenuItem> _menuItems = new List<MenuItem>();

        //gets or sets whether the menu is active
        public bool IsActive { get; set; }

        //derived classes must implement this to create the menu
        public abstract void CreateMenu();

        /// displays the menu and handles user selection
        public override void Select()
        {
            IsActive = true;
            do
            {
                CreateMenu();
                string output = $"{MenuText()}{Environment.NewLine}";
                int selection = ConsoleHelper.GetIntegerInRange(1, _menuItems.Count, ToString()) - 1;
                _menuItems[selection].Select();
            } while (IsActive);
        }

        // returns the menu text and items as a string
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(MenuText());
            for (int i = 0; i < _menuItems.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {_menuItems[i].MenuText()}");
            }
            return sb.ToString();
        }
    }
}
