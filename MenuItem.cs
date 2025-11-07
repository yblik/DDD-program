using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal abstract class MenuItem
    {
        //does the menu item action
        public abstract void Select();

        //displays the menu item text
        public abstract string MenuText();
    }
}
