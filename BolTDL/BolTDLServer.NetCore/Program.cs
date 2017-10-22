using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RedHttpServerCore;
using RedHttpServerCore.Plugins;


namespace BolTDLServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var server = new RedHttpServer(3000, "www");
            var logger = new TerminalLogging();

            server.Get("/", async (req, res) =>
            {
                await res.SendString("Hello, World!");
            });

            server.Get("/:route", async (req, res) =>
            {
                //Logger.Log("User tried to access " + req.Params["route"]);
                await res.SendString("Helo, 404!");
            });

            server.Post("/getuser", async (req, res) =>
            {
                var post = await req.GetFormDataAsync();
                string username = post["username"][0];
                string pass = post["pass"][0];

                await res.SendFile($"www/{username}.txt");

            });

            server.Post("/createuser", async (req, res) =>
            {

                var post = await req.GetFormDataAsync();
                string username = post["username"][0];
                string pass = post["password"][0];

                if (Directory.GetFiles("./", $"{username}-*").Length > 0)
                {
                    await res.SendString("User already exists, sorry!.");
                }
                else
                {
                    File.WriteAllText($"{username}-{pass}.txt", "[]");
                    await res.SendString($"Created user {username} with pass {pass}");
                    logger.Log("Created user " + username);
                }
            });

            server.Post("/getdata", async (req, res) =>
            {
                var post = await req.GetFormDataAsync();
                string username = post["username"];
                string pass = post["password"];

                //Logger.Log($"User {username} accesing get data using pass {pass}");
                if (UserExists(username) && CorrectLoginInfo(username, pass))
                {
                    await res.SendFile($"./{username}-{pass}.txt", "text/plain");
                }
                else
                {
                    await res.SendString($"Wrong username/password.");
                }
            });

            server.Post("/setdata", async (req, res) =>
            {
                var post = await req.GetFormDataAsync();
                string username = post["username"];
                string pass = post["password"];
                string savedata = post["savedata"];
                logger.Log($"User {username} updated list!");

                if (UserExists(username) && CorrectLoginInfo(username, pass))
                {
                    using (FileStream steam = File.Create(LocalAddressForUser(username, pass)))
                    {
                        byte[] info;
                        info = new System.Text.UTF8Encoding(true).GetBytes(savedata);
                        steam.Write(info, 0, info.Length);
                    }
                    await res.SendString("Sucess");
                }
                else
                {
                    await res.SendString("Error");
                }
                
            });

            server.Get("/register", async (req, res) =>
            {
                await res.RenderPage("www/index.ecs", null);
            });


            server.Start();
        }

        private static bool UserExists(string username)
        {
            return Directory.GetFiles("./", $"{username}-*").Length > 0;
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