using System;
using LiteDB;

namespace BolTDLServer.NetCore
{
    public class Item
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; }
        public string Description { get; set; }
    }
}