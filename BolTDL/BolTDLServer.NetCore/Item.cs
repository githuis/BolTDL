using System;
using LiteDB;

namespace BolTDLServer.NetCore
{
    public class Item : IModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }
}