using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program.MenuLogic
{
    public abstract class MenuItem
    {
        //public int Role { get; }

        //protected MenuItem(int role)  //tried and failed to prevent code repeating so gave up and will repeat like a good boy
        //{
        //    Role = role;
        //}
        //does the menu item action
        public abstract void Select();

        //displays the menu item text
        public abstract string MenuText();
    }
}
