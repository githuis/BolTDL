using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace BolTDLServer.NetCore
{
    public class LiteDatabase : IDatabase
    {
        private readonly LiteCollection<Category> _categoryTable;
        private readonly LiteCollection<User> _userTable;

        public LiteDatabase(string connectionString = "db.lite.db")
        {
            BsonMapper.Global.Entity<Item>()
                .Id(x => x.Id);

            BsonMapper.Global.Entity<Category>()
                .Id(x => x.Id);

            BsonMapper.Global.Entity<User>()
                .Id(x => x.Username);

            _categoryTable = new LiteDB.LiteDatabase(connectionString).GetCollection<Category>();
            _userTable = new LiteDB.LiteDatabase(connectionString).GetCollection<User>();
        }

        public bool InsertUser(User user)
        {
            return _userTable.Insert(user);
        }

        public bool UpdateUser(User user)
        {
            return _userTable.Update(user);
        }

        public bool DeleteUser(string username)
        {
            return _userTable.Delete(Query.EQ(nameof(User.Username), username)) == 1;
        }

        public User FindUser(Expression<Func<User, bool>> predicate)
        {
            return _userTable.FindOne(predicate);
        }

        
        public bool InsertCategory(Category category)
        {
            return _categoryTable.Insert(category);
        }

        public bool UpdateCategory(Category category)
        {
            return _categoryTable.Update(category);
        }

        public bool DeleteCategory(string categoryId)
        {
            return _categoryTable.Delete(Query.EQ(nameof(Category.Id), categoryId)) == 1;
        }

        public IEnumerable<Category> FindCategories(Expression<Func<Category, bool>> predicate)
        {
            return _categoryTable.Find(predicate);
        }
    }
}