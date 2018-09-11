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
using Common.Web;
using PI.Models.Composite;

namespace PI.Core
{
    public interface IEstadisticaManager : IManager<ExpedientesByArea> 
    {
        PagedList GetEstadisticasByArea(int year);
        PagedList GetIngresoExpedientesPorMes(int year);
    }

    public class EstadisticaManager : Manager<ExpedientesByArea>, IEstadisticaManager
    {
        public EstadisticaManager(IEstadisticaRepository repository, ITransaction transaction) : base(repository, transaction) 
        { 
        }

        public PagedList GetEstadisticasByArea(int year)
        {
            var PagedList = ((IEstadisticaRepository)_repository).GetEstadisticasByArea(year);
            return PagedList;
        }

        public PagedList GetIngresoExpedientesPorMes(int year)
        {
            var PagedList = ((IEstadisticaRepository)_repository).GetIngresoExpedientesPorMes(year);
            return PagedList;
        }
    }

}
