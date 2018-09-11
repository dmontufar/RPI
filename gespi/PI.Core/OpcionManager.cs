using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class OpcionManager : Manager<Opciones>, IOpcionManager
    {
        public OpcionManager(IOpcionRepository repository, ITransaction transaction) : base(repository, transaction) { }

        public IList<OpcionesSiguientes> GetOpcionesPorEstatus(int estadoActualId, int leyId = 2) 
        {
            var result = ((IOpcionRepository)Repository).GetOpcionesPorEstatus(estadoActualId, leyId);
            return result;
        }
    }
}
