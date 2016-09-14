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
            CLListNavigator nav = new CLListNavigator(DataHandler.Load());
            nav.LoadSettings();
            Console.Title = "BolTDL";
            nav.PrintList();
        }
    }
}
