using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public interface IAuditoriaRepository : IRepository<Auditoria> { }

    public class AuditoriaRepository : Repository<Auditoria>, IAuditoriaRepository
    {
        public AuditoriaRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }
}
