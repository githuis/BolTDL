using System;
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

            RHttpServer.Logging.Logger.Configure (RHttpServer.Logging.LoggingOption.Terminal, true);

            server.Get("/", (req, res) =>
            {
                res.SendString("Helo, World!");
            });

            server.Get("/*", (req, res) =>
            {
                res.SendString("Helo, 404!");
            });


            server.Start(true);
        }
    }
}