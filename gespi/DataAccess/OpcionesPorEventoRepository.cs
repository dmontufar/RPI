using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class OpcionesPorEventoRepository : Repository<OpcionesPorEvento>, IOpcionesPorEventoRepository
    {
        public OpcionesPorEventoRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public override PagedList GetPage<TOrder>(int pageNumber, int pageSize, Expression<Func<OpcionesPorEvento, bool>> where, Expression<Func<OpcionesPorEvento, TOrder>> order)
        {
            var result = new PagedList();
            try
            {
                // temporarily here
                // fixes error: A circular reference was detected while serializing an object of type 
                DbContext.Configuration.ProxyCreationEnabled = false;

                var results = (where != null) ?
                            DbContext.OpcionesPorEventos
                            .OrderBy(order).Where(where).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                            :
                            DbContext.OpcionesPorEventos
                            .OrderBy(order).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                result.TotalItems = (where != null) ?
                    DbContext.OpcionesPorEventos.Where(where).Count()
                    : DbContext.OpcionesPorEventos.Count();


                var r2 = results.Select(oxe => new OpsXEvento()
                {
                    Id = oxe.Id,
                    EstatusActualId = oxe.EstatusActualId,
                    OpcionId = oxe.OpcionId,
                    Estatus = DbContext.Estatus.Where(u => u.Id == oxe.EstatusActualId).First().Descripcion,
                    Opcion = DbContext.Opciones.Where(x => x.Id == oxe.OpcionId).First().Descripcion
                }).ToList();


                result.DataSet = r2;

                //result.TotalItems = results == null ? 0 : total;

                var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

                result.HasPreviousPage = pageNumber > 1;
                result.HasNextPage = pageNumber < pageCount;
                result.IsFirstPage = pageNumber == 1;
                result.IsLastPage = pageNumber >= pageCount;
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                System.Diagnostics.Trace.TraceError(exception.Message);
                System.Diagnostics.Trace.TraceError(where.ToString());
                System.Diagnostics.Trace.TraceError(pageNumber.ToString());
                System.Diagnostics.Trace.TraceError(pageSize.ToString());
            }
            return result;
        }

    }
}
