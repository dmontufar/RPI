using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class EventoRepository : Repository<Eventos>, IEventoRepository
    {
        public EventoRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }
}
