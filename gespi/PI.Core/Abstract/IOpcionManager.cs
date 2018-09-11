using PI.Common;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IOpcionManager : IManager<Opciones>
    {
        IList<OpcionesSiguientes> GetOpcionesPorEstatus(int estadoActualId, int leyId=2);
    }
}
