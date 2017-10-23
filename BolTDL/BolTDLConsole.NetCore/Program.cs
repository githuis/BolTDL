using System;
using BolTDLCore.NetStandard;

namespace BolTDLConsole.NetCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var nav = new ClListNavigator(DataHandler.ListLoad());
            if (nav.LoadSettings())
            {
                try
                {
                    nav = new ClListNavigator(DataHandler.ListLoadWeb(nav.GetWebHost, nav.GetUsername,
                        nav.GetPassword));
                    nav.LoadSettings();
                    var title = $"BolTDL - Todo list for {nav.GetUsername}";
                    Console.Title = title;
                    nav.SetOldTitle(title);
                    nav.PrintList();
                }
                catch (AggregateException aex)
                {
                    if (aex.InnerException is DownloadException)
                    {
                        Console.WriteLine(
                            "Error downloading list. Please check your internet connection or disable websync in settings.");
                        Console.WriteLine("\nPress any key to quit...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured. This is the message: " + ex.Message);
                    Console.WriteLine("\nPress any key to quit...");
                    Console.ReadKey();
                }
            }
            else
            {
                var title = $"BolTDL - Todo list for {nav.GetUsername}";
                Console.Title = title;
                nav.SetOldTitle(title);
                nav.PrintList();
            }
        }
    }
}