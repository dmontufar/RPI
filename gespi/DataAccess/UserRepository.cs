using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class UserRepository : Repository<Usuario>, IUserRepository
    {
        public UserRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class UsuarioPublicoRepository : Repository<UsuarioPublico>, IUsuarioPublicoRepository
    {
        public UsuarioPublicoRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

}
