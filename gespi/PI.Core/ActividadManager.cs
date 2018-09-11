using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    //TODO:REMOVE
    //public class ActividadManager : Manager<Actividad>, IActividadManager
    //{
    //    public ActividadManager(IActividadRepository repository, ITransaction transaction) : base(repository, transaction) {}

    //    public ResultInfo GetPosiblesActividadesSiguientes(int actividadActualId, int pageNumber, int pageSize)
    //    {
    //        var resultInfo = new ResultInfo();
    //        resultInfo.Result = ((IActividadRepository)Repository).GetPosiblesActividadesSiguientes(actividadActualId, pageNumber, pageSize);
    //        resultInfo.Succeeded = true;

    //        return resultInfo;
    //    }

    //    public IList GetActividadesSiguientes(int actividadActualId, int estadoActualId)
    //    {
    //        var result = ((IActividadRepository)Repository).GetActividadesSiguientes(actividadActualId, estadoActualId);
    //        return result;
    //    }
    //}
}
