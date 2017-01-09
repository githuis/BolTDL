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
		public static void Main (string[] args)
		{
			var server = new HttpServer (3000, 4, "www");

			RHttpServer.Logging.Logger.Configure (RHttpServer.Logging.LoggingOption.Terminal, true);

			server.Get ("/", (req, res) => {
				res.SendString ("Helo, World!");
			});

			server.Get ("/*", (req, res) => {
				res.SendString ("Helo, 404!");
			});

			server.Post ("/getuser", (req, res) => {
				var post = req.GetBodyPostFormData ();
				string username = post ["username"];
				string pass = post ["pass"];

				res.SendFile ($"www/{username}.txt");

			});

			server.Post ("/createuser", (req, res) => {

				var post = req.GetBodyPostFormData ();
				string username = post ["username"];
				string pass = post ["password"];

				if (Directory.GetFiles ("./", $"{username}-*").Length > 0 ) {
					//var files = Directory.GetFiles("./", "{username}-*");
					//Logger.Log(files.ToString());
					res.SendString ("User already exists, sorry!.");
				} else {
					File.Create ($"{username}-{pass}.txt");
					res.SendString ($"Created user {username} with pass {pass}");
				}
				//if(Directory.CreateDirectory("./username"))

			});

			server.Post ("/getdata", (req, res) => {
				var post = req.GetBodyPostFormData ();
				string username = post ["username"];
				string pass = post ["password"];

				if (Directory.GetFiles ("./", $"{username}-*").Length > 0 ) {
					res.SendFile ($"./{username}-{pass}.txt", "text/plain");
					return;
				}

				res.SendString ("meeeeeemes");
			});
		
			server.Get ("/register", (req, res) => {
				Logger.Log ("Page reached");
				res.RenderPage ("www/index.ecs", new RHttpServer.Response.RenderParams ());
			});


			server.Start (true);
		}
	}
}