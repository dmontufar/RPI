using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    //TODO:REMOVE!
    //public class ActividadRepository : Repository<Actividad>, IActividadRepository
    //{
    //    public ActividadRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

    //    public PagedList GetPosiblesActividadesSiguientes(int actividadActualId, int pageNumber, int pageSize)
    //    {
    //        var resultset = (from r in DbContext.Reglas
    //                         join a in DbContext.Actividades on r.ActividadActualId equals a.Id
    //                         join e in DbContext.Estatus on r.EstadoActualId equals e.Id
    //                         join a2 in DbContext.Actividades on r.ActividadAEjecutarId equals a2.Id
    //                         where r.ActividadActualId == actividadActualId
    //                         select new { 
    //                                 Id = r.Id,
    //                                 ActividadActual = a.Descripcion,
    //                                 EstadoActual = e.Descripcion,
    //                                 ActividadSiguiente = a2.Descripcion,
    //                            });

    //        var result = new PagedList();

    //        result.DataSet = resultset.OrderBy(o=>o.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    //        result.TotalItems = resultset.Count();

    //        var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

    //        result.HasPreviousPage = pageNumber > 1;
    //        result.HasNextPage = pageNumber < pageCount;
    //        result.IsFirstPage = pageNumber == 1;
    //        result.IsLastPage = pageNumber >= pageCount;

    //        return result;
    //    }

    //    public IList GetActividadesSiguientes(int actividadActualId, int estadoActualId)
    //    {
    //        var resultset = (from r in DbContext.Reglas
    //                         join a2 in DbContext.Actividades on r.ActividadAEjecutarId equals a2.Id
    //                         where r.ActividadActualId == actividadActualId && r.EstadoActualId == estadoActualId
    //                         select new
    //                         {
    //                             Id = r.Id,
    //                             ActividadSiguiente = a2.Descripcion,
    //                             Codigo = a2.Codigo
    //                         }).ToList();
    //        //SELECT AE.Codigo
    //        //FROM gpi_workflow.Reglas R
    //        //INNER JOIN gpi_workflow.Actividades AE ON AE.Id = R.ActividadAEjecutarId
    //        //WHERE R.ActividadActualId=4 AND R.EstadoActualId = 1

    //        return resultset;
    //    }
    //}
}
