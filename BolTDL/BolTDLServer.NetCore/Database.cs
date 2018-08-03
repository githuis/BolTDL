using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace BolTDLServer.NetCore
{
    public class LiteDatabase : IDatabase
    {
        private readonly LiteDB.LiteDatabase _db;

        public LiteDatabase(string connectionString = "db.lite.db")
        {
            BsonMapper.Global.Entity<Item>()
                .Id(x => x.Id);

            BsonMapper.Global.Entity<User>()
                .Id(x => x.Id);

           _db = new LiteDB.LiteDatabase(connectionString);
        }

        public bool Insert<T>(T model) where T : IModel => _db.GetCollection<T>().Insert(model);
        public bool Update<T>(T model) where T : IModel => _db.GetCollection<T>().Update(model);
        public bool Delete<T>(T model) where T : IModel => _db.GetCollection<T>().Delete(model.Id);
        public IEnumerable<T> Get<T>() where T : IModel => _db.GetCollection<T>().FindAll();
        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IModel => _db.GetCollection<T>().Find(predicate);
        public T FindOne<T>(Expression<Func<T, bool>> predicate) where T : IModel => _db.GetCollection<T>().FindOne(predicate);

    }
}