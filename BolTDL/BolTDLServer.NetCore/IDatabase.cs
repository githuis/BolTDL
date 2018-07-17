using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BolTDLServer.NetCore
{
    public interface IDatabase
    {
        
        bool InsertUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(string username);
        User FindUser(Expression<Func<User, bool>> predicate);
        
        bool InsertCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(string categoryId);
        IEnumerable<Category> FindCategories(Expression<Func<Category, bool>> predicate);
    }
}