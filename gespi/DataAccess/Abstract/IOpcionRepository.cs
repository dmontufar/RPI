using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IOpcionRepository : IRepository<Opciones>
    {
        IList<OpcionesSiguientes> GetOpcionesPorEstatus(int estadoActualId, int leyId=2);
    }
}
