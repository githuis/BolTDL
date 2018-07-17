using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Red;
using Red.CookieSessions;
using Red.Extensions;

namespace BolTDLServer.NetCore
{
    class Session
    {
        public string Username { get; set; }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var server = new RedHttpServer(3000, "www");
            var db = new LiteDatabase();

            async Task Auth(Request req, Response res)
            {
                var session = req.GetSession<Session>();
                if (session == null)
                {
                    await res.SendStatus(HttpStatusCode.Unauthorized);
                }
            }

            server.Post("/login", async (req, res) =>
            {
                var form = await req.GetFormDataAsync();

                string username = form["username"];
                string password = form["password"];

                var user = db.FindUser(u => u.Username == username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    await res.SendStatus(HttpStatusCode.Unauthorized);
                    return;
                }
                
                req.OpenSession(new Session{Username = username});
                await res.SendStatus(HttpStatusCode.OK);
            });
            server.Post("/logout", Auth, async (req, res) =>
            {
                req.GetSession<Session>().Close(req);
                await res.SendStatus(HttpStatusCode.OK);
            });
            
            
            server.Post("/category", Auth, async (req, res) =>
            {
                var sessionData = req.GetSession<Session>().Data;
                var category = await req.ParseBodyAsync<Category>();
                if (category == null)
                {
                    await res.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }
                category.Owner = sessionData.Username;
                category.Items = new List<Item>();
                if (db.InsertCategory(category))
                {
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.InternalServerError);
                }
            });


            server.Post("/category/:categoryId", Auth, async (req, res) =>
            {
                var sessionData = req.GetSession<Session>().Data;
                var category = db.FindCategories(c => c.Id == req.Parameters["categoryId"]).FirstOrDefault();
                if (category == null || category.Owner != sessionData.Username)
                {
                    await res.SendStatus(HttpStatusCode.NotFound);
                    return;
                }
                var item = await req.ParseBodyAsync<Item>();
                if (item == null)
                {
                    await res.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }
                
                category.Items.Add(item);
                if (db.UpdateCategory(category))
                {
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.InternalServerError);
                }
            });

            server.Get("/categories", Auth, async (req, res) =>
            {
                var sessionData = req.GetSession<Session>().Data;
                var categories = db.FindCategories(c => c.Owner == sessionData.Username);
                await res.SendJson(categories);

            });

            server.Post("/user", async (req, res) =>
            {
                var form = await req.GetFormDataAsync();
                string username = form["username"];
                string password = form["password"];

                if (db.FindUser(u => u.Username == username) != null)
                {
                    await res.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                var user = new User
                {
                    Username = username,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                };
                
                if (db.InsertUser(user))
                {
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.BadRequest);
                }
            });

            server.Start();
        }
    }
}