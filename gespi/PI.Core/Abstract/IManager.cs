using PI.Common;
using PI.DataAccess;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IManager<T> where T : class
    {
        ResultInfo Create(T entity);
        ResultInfo Update(T entity);
        ResultInfo Delete(int id);
        ResultInfo Get(int id);
        ResultInfo GetPage<TOrder>(int pageNumber, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);
    }
}
