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
			if(nav.LoadSettings())
			{
				nav = new CLListNavigator (DataHandler.ListLoadWeb ($"{nav.GetWebHost}"));	
			}
			Console.Title = $"BolTDL - Todo list for {nav.GetUsername}";
            nav.PrintList();
        }
    }
}
