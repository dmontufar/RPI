using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    //TODO:REMOVE
    //public class ReglaDataFormRepository : Repository<ReglaDataForm>, IReglaDataFormRepository
    //{
    //    public ReglaDataFormRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

    //    public PagedList GetForma(int reglaId, int pageNumber, int pageSize)
    //    {
    //        var resultset = (from f in DbContext.ReglaDataForm
    //                         join tp in DbContext.TipoDeDatos on f.TypeId equals tp.Id
    //                         where f.ReglaId == reglaId
    //                         select new
    //                         {
    //                             Id = f.Id,
    //                             Etiqueta = f.Etiqueta,
    //                             Tipo = tp.Tipo,
    //                             Obligatorio = f.EsRequerido?"Si": "No"
    //                         });

    //        var result = new PagedList();

    //        result.DataSet = resultset.OrderBy(o => o.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    //        result.TotalItems = resultset.Count();

    //        var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

    //        result.HasPreviousPage = pageNumber > 1;
    //        result.HasNextPage = pageNumber < pageCount;
    //        result.IsFirstPage = pageNumber == 1;
    //        result.IsLastPage = pageNumber >= pageCount;

    //        return result;
    //    }
    //}
}
