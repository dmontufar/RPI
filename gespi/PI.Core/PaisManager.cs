using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class PaisManager : Manager<Pais>, IPaisManager
    {
        public PaisManager(IPaisRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }
}

//    public class PaisManager : IPaisManager 
//    {
//        private IPaisRepository _repository;
//        private ITransaction _transaction;
//        private ResultInfo _result;

//        public PaisManager(IPaisRepository paisRepository, ITransaction transaction) 
//        {
//            _repository = paisRepository;
//            _transaction = transaction;
//            _result = new ResultInfo();
//        }

//        public ResultInfo Create(Pais pais)
//        {
//            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
//            {
//                pais = _repository.Add(pais);
//                return new ResultInfo() { Succeeded = true, Result = pais };
//            });
//        }

//        public ResultInfo Update(Pais pais)
//        {
//            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
//            {
//                pais = _repository.Update(pais);
//                return new ResultInfo() { Succeeded = true, Result = pais };
//            });
//        }

//        public ResultInfo Delete(int id)
//        {
//            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
//            {
//                _repository.Delete(p=>p.Id==id);
//                return new ResultInfo() { Succeeded = true };
//            });
//        }

//        public ResultInfo Get(int id)
//        {
//            var pais = _repository.GetById(id);
//            return new ResultInfo() { Succeeded = true, Result = pais };
//        }

//        public ResultInfo GetPage<TOrder>(int pageNumber, int pageSize, Expression<Func<Pais, bool>> where, Expression<Func<Pais, TOrder>> order)
//        {
//            var pagedData = _repository.GetPage(pageNumber, pageSize, where, order);
//            return new ResultInfo() { Succeeded = true, Result = pagedData };
//        }

//    }
//}
