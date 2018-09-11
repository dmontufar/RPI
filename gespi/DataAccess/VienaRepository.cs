using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class VienaRepository : Repository<ClasificacionDeViena>, IVienaRepository
    {
        public VienaRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }
}
