using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public interface IRepository<T> where T: class
    {
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(long id);
        T GetById(string id);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        PagedList GetPage<TOrder>(int pageNumber, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);
    }
}
