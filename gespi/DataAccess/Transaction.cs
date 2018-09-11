using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PI.DataAccess
{
    public class Transaction : ITransaction
    {
        //private readonly IDatabaseFactory databaseFactory;
        private GPIEntities _context;

        public Transaction(IDatabaseFactory databaseFactory)
        {
            _context = _context ?? databaseFactory.MakeDbContext();
        }
        
        protected GPIEntities DataContext
        {
            get { return _context; }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        //public ResultInfo 
        public TServiceResult ExecuteInTransactionScope<TServiceResult>(Func<TServiceResult> script) where TServiceResult : class, new()
        {
            TServiceResult serviceResult = default(TServiceResult);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    serviceResult = script();
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Trace.TraceError(exception.Message);
                    transaction.Rollback();
                }
            }

            return serviceResult;
        }


    }
}
