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
        public Guid Id { get; set; }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var server = new RedHttpServer(3000, "D:\\Daniel\\BolTDL-Webapp\\dist");
            var db = new LiteDatabase();

            server.Use(new CookieSessions<Session>(new CookieSessionSettings(TimeSpan.FromDays(14))
            {
                Secure = false
            }));

            server.Post("/api/login", async (req, res) =>
            {
                var form = await req.GetFormDataAsync();

                string username = form["username"];
                string password = form["password"];

                var user = db.FindOne<User>(u => u.Username == username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    await res.SendStatus(HttpStatusCode.NotFound);
                    return;
                }
                
                req.OpenSession(new Session{Id = user.Id});
                await res.SendStatus(HttpStatusCode.OK);
            });
            server.Post("/api/logout", Auth, async (req, res) =>
            {
                req.GetSession<Session>().Close(req);
                await res.SendStatus(HttpStatusCode.OK);
            });
            
            
   
            server.Post("/api/register", async (req, res) =>
            {
                var form = await req.GetFormDataAsync();
                string username = form["username"];
                string password = form["password"];

                var x = db.FindOne<User>(u => u.Username == username);
                if (x != null)
                {
                    await res.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                var user = new User
                {
                    Username = username,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                };
                
                if (db.Insert(user))
                {
                    req.OpenSession(new Session {Id = user.Id});
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.BadRequest);
                }
            });
            
            server.Get("/api/session", Auth, async (req, res) => await res.SendStatus(HttpStatusCode.OK));
            
            server.Get("/api/items", Auth, async (req, res) =>
            {
                var sessionData = req.GetSession<Session>().Data;
                var items = db.Find<Item>(c => c.OwnerId == sessionData.Id);
                await res.SendJson(items);
            });
            
            server.Post("/api/item", Auth, CanParse<Item>, async (req, res) =>
            {
                var item = req.GetData<Item>();

                var id = req.GetSession<Session>().Data.Id;
                item.OwnerId = id;
                
                if (db.Insert(item))
                {
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.InternalServerError);
                }
            });

            server.Put("/api/item", Auth, CanParse<Item>, async (req, res) =>
            {
                var item = req.GetData<Item>();

                var id = req.GetSession<Session>().Data.Id;
                if (item.OwnerId != id)
                {
                    await res.SendStatus(HttpStatusCode.Unauthorized);
                    return;
                }

                var existing = db.FindOne<Item>(i => i.Id == item.Id);
                if (existing == null)
                {
                    await res.SendStatus(HttpStatusCode.NotFound);
                    return;
                }
                
                if (db.Update(item))
                {
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.InternalServerError);
                }
            });
            
            server.Delete("/api/item", Auth, CanParse<Item>, async (req, res) =>
            {
                var item = req.GetData<Item>();

                var id = req.GetSession<Session>().Data.Id;
                if (item.OwnerId != id)
                {
                    await res.SendStatus(HttpStatusCode.Unauthorized);
                    return;
                }
                
                if (db.Delete(item))
                {
                    await res.SendStatus(HttpStatusCode.OK);
                }
                else
                {
                    await res.SendStatus(HttpStatusCode.InternalServerError);
                }
            });

            
            server.Start();
            Console.Read();
        }
        
        private static async Task Auth(Request req, Response res)
        {
            var session = req.GetSession<Session>();
            if (session == null)
            {
                await res.SendStatus(HttpStatusCode.Unauthorized);
            }
        }
        private static async Task CanParse<T>(Request req, Response res)
        {
            var model = await req.ParseBodyAsync<T>();
            if (model == null)
            {
                await res.SendStatus(HttpStatusCode.BadRequest);
                return;
            }
            req.SetData(model);
        }
    }
}