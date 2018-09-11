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
    public class MarcaManager : Manager<Marca>, IMarcaManager
    {
        public MarcaManager(IMarcaRepository repository, ITransaction transaction) : base(repository, transaction) { }

        public ResultInfo searchTitular(string textToSearch)
        {
            var pagedData = ((IMarcaRepository)_repository).searchTitular(textToSearch);
            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }
    }
}
