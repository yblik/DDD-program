// See https://aka.ms/new-console-template for more information
using DDD_program;
using System.Xml.Linq;

Console.WriteLine("Welcome to the system");


RoleLog RL = new RoleLog();

//login prerequisite
SQLstorage.Initialize();
RL.Login();

