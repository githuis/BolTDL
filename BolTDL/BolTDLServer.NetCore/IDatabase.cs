using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BolTDLServer.NetCore
{
    public interface IDatabase
    {
        bool Insert<T>(T model) where T : IModel;
        bool Update<T>(T model) where T : IModel;
        bool Delete<T>(T model) where T : IModel;
        IEnumerable<T> Get<T>() where T : IModel;
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IModel;
        T FindOne<T>(Expression<Func<T, bool>> predicate) where T : IModel;
    }
}