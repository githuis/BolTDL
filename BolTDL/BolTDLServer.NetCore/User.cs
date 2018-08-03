using System;

namespace BolTDLServer.NetCore
{
    public class User : IModel
    {
        public Guid Id { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }
    }
}