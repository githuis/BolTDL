using System;
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

				res.SendFile($"www/{username}.txt");

			});

			server.Post ("/createuser", (req, res) => {
				res.SendString("Hello!");
			});

			server.Get ("/register", (req, res) => {
				Logger.Log("Page reached");
				res.RenderPage("www/index.ecs", new RHttpServer.Response.RenderParams());
			});


			server.Start (true);
		}
	}
}