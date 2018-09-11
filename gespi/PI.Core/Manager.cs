using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PI.DataAccess;

namespace PI.Core
{
    public class Manager<T> : IManager<T> where T : class
    {
        public IRepository<T> _repository;
        public ITransaction _transaction;
        private ResultInfo _result;

        public Manager(IRepository<T> repository, ITransaction transaction)
        {
            _repository = repository;
            _transaction = transaction;
            _result = new ResultInfo();
        }

        public IRepository<T> Repository
        {
            get { return _repository; }
        }

        public virtual ResultInfo Create(T entity)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                entity = _repository.Add(entity);
                return new ResultInfo() { Succeeded = true, Result = entity };
            });
        }

        public ResultInfo Update(T entity)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                entity = _repository.Update(entity);
                return new ResultInfo() { Succeeded = true, Result = entity };
            });
        }

        public ResultInfo Delete(int id)
        {

            var parameter = Expression.Parameter(typeof(int));
            var property = Expression.Property(parameter, "Id");
            var target = Expression.Constant(id);
            var containsMethod = Expression.Call(property, "equals", null, target);
            var lambda = Expression.Lambda<Func<T, bool>>(containsMethod, parameter);

            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                _repository.Delete(lambda);
                return new ResultInfo() { Succeeded = true };
            });

            //return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            //{
            //    _repository.Delete(p => p.Id == id);
            //    return new ResultInfo() { Succeeded = true };
            //});
        }

        public ResultInfo Get(int id)
        {
            var entity = _repository.GetById(id);
            return new ResultInfo() { Succeeded = true, Result = entity };
        }

        //pageNumber == 0 >> all records
        public ResultInfo GetPage<TOrder>(int pageNumber, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
        {
            var pagedData = _repository.GetPage(pageNumber, pageSize, where, order);
            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }
    }
}
