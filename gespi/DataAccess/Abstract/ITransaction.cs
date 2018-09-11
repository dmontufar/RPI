using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface ITransaction
    {
        void Commit();
        TServiceResult ExecuteInTransactionScope<TServiceResult>(Func<TServiceResult> script) where TServiceResult : class, new();
    }
}
