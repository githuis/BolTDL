using System;
using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace BolTDLServer.NetCore
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; }
        public List<Item> Items { get; set; }
        [JsonIgnore]
        public string Owner { get; set; }
    }
}