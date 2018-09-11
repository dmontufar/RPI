using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class OpcionRepository : Repository<Opciones>, IOpcionRepository
    {
        public OpcionRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public IList<OpcionesSiguientes> GetOpcionesPorEstatus(int estadoActualId, int leyId=2) 
        {
            var resultset = (from oxe in DbContext.OpcionesPorEventos
                             join o in DbContext.Opciones on oxe.OpcionId equals o.Id
                             join e in DbContext.Eventos on oxe.EventoId equals e.Id
                             where oxe.EstatusActualId == estadoActualId && e.LeyId == leyId
                             select new OpcionesSiguientes()
                             {
                                 Id = oxe.Id,
                                 Descripcion = o.Descripcion,
                                 Opcion = o.Opcion,
                                 style = o.style
                             }).ToList();
            return resultset;
        }

        //public IList GetOpcionesPorEvento(int estadoActualId)
        //{
        //}
    }
}
