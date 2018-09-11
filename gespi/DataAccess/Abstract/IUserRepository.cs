using PI.Common;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IUserRepository : IRepository<Usuario> {}

    public interface IUsuarioPublicoRepository : IRepository<UsuarioPublico> { }

}
