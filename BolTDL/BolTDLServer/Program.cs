using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using RHttpServer;
using RHttpServer.Logging;


namespace BolTDLServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var server = new HttpServer(3000, 4, "www");

            RHttpServer.Logging.Logger.Configure(RHttpServer.Logging.LoggingOption.Terminal, true);

            server.Get("/", (req, res) =>
            {
                res.SendString("Helo, World!");
            });

            server.Get("/:route", (req, res) =>
            {

                //Logger.Log("User tried to access " + req.Params["route"]);
                res.SendString("Helo, 404!");
            });

            server.Post("/getuser", (req, res) =>
            {
                var post = req.GetBodyPostFormData();
                string username = post["username"];
                string pass = post["pass"];

                res.SendFile($"www/{username}.txt");

            });

            server.Post("/createuser", (req, res) =>
            {

                var post = req.GetBodyPostFormData();
                string username = post["username"];
                string pass = post["password"];

                if (Directory.GetFiles("./", $"{username}-*").Length > 0)
                {
                    res.SendString("User already exists, sorry!.");
                }
                else
                {
                    File.WriteAllText($"{username}-{pass}.txt", "[]");
                    res.SendString($"Created user {username} with pass {pass}");
                    Logger.Log("Created user " + username);
                }
            });

            server.Post("/getdata", (req, res) =>
            {
                var post = req.GetBodyPostFormData();
                string username = post["username"];
                string pass = post["password"];

                //Logger.Log($"User {username} accesing get data using pass {pass}");
                if (UserExists(username) && CorrectLoginInfo(username, pass))
                {
                    res.SendFile($"./{username}-{pass}.txt", "text/plain");
                }
                else
                {
                    res.SendString($"Wrong username/password.");
                }
            });

            server.Post("/setdata", (req, res) =>
            {
                var post = req.GetBodyPostFormData();
                string username = post["username"];
                string pass = post["password"];
                string savedata = post["savedata"];
                Logger.Log($"User {username} updated list!");

                if (UserExists(username) && CorrectLoginInfo(username, pass))
                {
                    using (FileStream steam = File.Create(LocalAddressForUser(username, pass)))
                    {
                        byte[] info;
                        info = new System.Text.UTF8Encoding(true).GetBytes(savedata);

                        steam.Write(info, 0, info.Length);
                    }

                    res.SendString("Sucess");
                }
                else
                    res.SendString("Error");
                
            });

            server.Get("/register", (req, res) =>
            {
                res.RenderPage("www/index.ecs", new RHttpServer.Response.RenderParams());
            });


            server.Start();
        }

        private static bool UserExists(string username)
        {
            return (Directory.GetFiles("./", $"{username}-*").Length > 0);
        }


        private static bool CorrectLoginInfo(string username, string password)
        {
            return File.Exists($"./{username}-{password}.txt");
        }

        private static string LocalAddressForUser(string username, string password)
        {
            return $"./{username}-{password}.txt";
        }
    }
}