using System;
using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace BolTDLServer.NetCore
{
    public class Category : IModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OwnerId { get; set; } = Guid.NewGuid();
        
        public string Name { get; set; }
    }
}