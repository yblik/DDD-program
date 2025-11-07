using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD_program
{
    internal class RoleLog //used to log role and kick start menus
    {
        public enum Role { student, Supervisor, SeniorTutor }
        public string Name;

        public int AgeOrStaffID; // for customer age or staff ID

        public Role SelectedProfile;



        public int RoleID; //bc csharp is retarded at times

        private bool selectedManager;
        private bool GOLD;
        public bool NONMEMBER;

        //for after login so roles are hardset
        public void SetProfile(int Selection)
        {
            if (Selection == 1)
            {
                SelectedProfile = Role.student;
                RoleID = 1;

                //TODO method to fetch username and find password
            }
            else if (Selection == 2)
            {
                SelectedProfile = Role.Supervisor;
                RoleID = 2;
                selectedManager = false; 
            }
            else
            {
                SelectedProfile = Role.SeniorTutor;
                RoleID = 3;
                selectedManager = true;
            }
        }
        public void Login()  //uses console helper
        {
            Name = ConsoleHelper.GetInput("Enter username");

            //password = SQLlite -> Username -> their password

            if (Name == "stu")
            {
                SetProfile(1);
            }
            else if(Name == "sup")
            {
                SetProfile(2);
            }
            else if (Name == "ST") 
            {  
                SetProfile(3); 
            }

            //for now this and no error checking

            //start menus
            


        }

    }
}
