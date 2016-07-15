using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BolTDL;

namespace BolTDLConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ToDoList list = DataHandler.Load();
            CLListNavigator nav = new CLListNavigator(list);
            Console.Title = "BolTDL";
            nav.PrintList();
        }
    }
}
