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
    //TODO:REMOVE
    //public class ReglaDataFormManager : Manager<ReglaDataForm>, IReglaDataFormManager
    //{
    //    public ReglaDataFormManager(IReglaDataFormRepository repository, ITransaction transaction)
    //        : base(repository, transaction) 
    //    { 
    //    }

    //    public ResultInfo GetForma(int reglaId, int pageNumber, int pageSize)
    //    {
    //        var resultInfo = new ResultInfo();
    //        resultInfo.Result = ((IReglaDataFormRepository)Repository).GetForma(reglaId, pageNumber, pageSize);
    //        resultInfo.Succeeded = true;

    //        return resultInfo;
    //    }

    //}
}
