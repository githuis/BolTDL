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
			CLListNavigator nav = new CLListNavigator(DataHandler.ListLoad());
            nav.LoadSettings();
			Console.Title = $"BolTDL - Todo list for {nav.GetUsername}";
            nav.PrintList();
        }
    }
}
