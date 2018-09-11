using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PI.DataAccess;
using Common.Web;
using PI.Models.Composite;

namespace PI.Core
{
    public interface ILeyesManager : IManager<Leyes> { }

    public class LeyesManager : Manager<Leyes>, ILeyesManager
    {
        public LeyesManager(ILeyesRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }


    public interface IPermisoManager : IManager<Permiso> 
    {
        ResultInfo SaveAll(List<Permiso> model);
    }

    public class PermisoManager : Manager<Permiso>, IPermisoManager
    {
        public PermisoManager(IPermisoRepository repository, ITransaction transaction) : base(repository, transaction) { }

        public ResultInfo SaveAll(List<Permiso> model) 
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                return ((IPermisoRepository)_repository).SaveAll(model);
            });
        }
    }

    public interface IRolManager : IManager<Rol> { }

    public class RolManager : Manager<Rol>, IRolManager
    {
        public RolManager(IRolRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }


    /// <summary>
    /// Session manager
    /// </summary>
    public interface ISessionManager : IManager<Session>
    {
        ResultInfo GetByToken(string token, int footMark);
        ResultInfo SetTokenForUserId(int userId, int footMark);
        ResultInfo isAuthorized(string token, int footMark, string perms);
    }

    public class SessionManager : Manager<Session>, ISessionManager
    {
        IPermisoRepository _permRepository;
        public SessionManager(ISessionRepository repository, IPermisoRepository permRepository, ITransaction transaction) : base(repository, transaction) 
        { 
            _permRepository = permRepository;
        }

        public ResultInfo GetByToken(string token, int footMark)
        {
            var session = ((ISessionRepository)_repository).Get(s => s.Token == token && s.FootMark == footMark);
            var result = new ResultInfo() { Result = session, Succeeded = session!=null };
            return result;
        }

        public ResultInfo isAuthorized(string token, int footMark, string perms)
        {
            var _perms = perms.Split(',');

            var session = ((ISessionRepository)_repository).Get(s => s.Token == token && s.FootMark == footMark);

            var autorized = false;
            if (session != null) {
                var usuarioId = session.UsuarioId;

                autorized = ((IPermisoRepository)_permRepository).GetMany(p => p.UsuarioId == usuarioId && _perms.Contains(p.Opcion)).Count()>0;            
            }
                        
            var result = new ResultInfo() { Result = session, Succeeded = session != null && autorized };
            return result;
        }


        public ResultInfo SetTokenForUserId(int userId, int footMark)
        {
            // get 1st random string 
            string key1 = Util.RandomString(4);
            // get 2nd random string 
            string key2 = Util.RandomString(4);

            // creat full rand string
            string token = key1 + "-" + key2;

            var session = ((ISessionRepository)_repository).Get(s => s.UsuarioId == userId && s.FootMark == footMark);
            if (session == null)
            {
                session = new Session();
            }
            session.UsuarioId = userId;
            session.TokenExp = DateTime.Now.AddDays(1);
            session.FootMark = footMark;

            if (session.Id == 0)
            {
                session.Token = token;
                ((ISessionRepository)_repository).Add(session);
            }
            else
            {
                ((ISessionRepository)_repository).Update(session);
            }
            var result = new ResultInfo() { Result = session, Succeeded = true };
            return result;
        }
    }


    public interface IPublicacionManager : IManager<Publicacion> { }

    public class PublicacionManager : Manager<Publicacion>, IPublicacionManager
    {
        public PublicacionManager(IPublicacionRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }


    public interface IFavoritoManager : IManager<Favorito> 
    {
        ResultInfo GetPage(int pageNumber, int pageSize, int? idGrupo, int idUsuario);
        ResultInfo Create(Favorito entity, int grupoId);
    }

    public class FavoritoManager : Manager<Favorito>, IFavoritoManager
    {
        public FavoritoManager(IFavoritoRepository repository, ITransaction transaction) : base(repository, transaction) {}

        public ResultInfo Create(Favorito entity, int grupoId)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                entity = ((IFavoritoRepository)_repository).Add(entity, grupoId);
                return new ResultInfo() { Succeeded = true, Result = entity };
            });
        }

        public ResultInfo GetPage(int pageNumber, int pageSize, int? idGrupo, int idUsuario)
        {
            var pagedData = ((IFavoritoRepository)_repository).GetPage(pageNumber, pageSize, idGrupo, idUsuario);
            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }
    }

    // Grupos creados por los usuarios externos!!
    public interface IGrupoManager : IManager<Grupo> { }

    public class GrupoManager : Manager<Grupo>, IGrupoManager
    {
        public GrupoManager(IGrupoRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }


    public interface IGrupoExpedienteManager : IManager<GrupoExpediente> { }

    public class GrupoExpedienteManager : Manager<GrupoExpediente>, IGrupoExpedienteManager
    {
        public GrupoExpedienteManager(IGrupoExpedienteRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }


    public interface IGrupoMiembroManager : IManager<GrupoMiembro> { }

    public class GrupoMiembroManager : Manager<GrupoMiembro>, IGrupoMiembroManager
    {
        public GrupoMiembroManager(IGrupoMiembroRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }

    /// <summary>
    /// Titulares de Patentes
    /// </summary>
    public interface IPatTitularManager : IManager<TitularEnPatentes> 
    {
        ResultInfo searchTitular(string textToSearch);
    }

    public class PatTitularManager : Manager<TitularEnPatentes>, IPatTitularManager
    {
        public PatTitularManager(IPatTitularRepository repository, ITransaction transaction) : base(repository, transaction) { }

        
        public ResultInfo searchTitular(string textToSearch)
        {
            var pagedData = ((IPatTitularRepository)_repository).searchTitular(textToSearch);
            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }
    }

    /// <summary>
    /// Agentes
    /// </summary>
    public interface IAgenteManager : IManager<Agente>
    {
        ResultInfo searchAgente(string textToSearch);
    }

    public class AgenteManager : Manager<Agente>, IAgenteManager
    {
        public AgenteManager(IAgenteRepository repository, ITransaction transaction) : base(repository, transaction) { }


        public ResultInfo searchAgente(string textToSearch)
        {
            var pagedData = ((IAgenteRepository)_repository).searchAgente(textToSearch);
            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }
    }

    /// <summary>
    /// Inventores
    /// </summary>
    public interface IInventorManager : IManager<Inventor> {
        ResultInfo SaveInventor(GenericEntity<Inventor> model, Auditoria auditoria);
        ResultInfo DeleteInventor(int inventorId, Auditoria auditoria);
    }

    public class InventorManager : Manager<Inventor>, IInventorManager
    {
        public InventorManager(IInventorRepository repository, ITransaction transaction) : base(repository, transaction) { }

        public ResultInfo SaveInventor(GenericEntity<Inventor> model, Auditoria auditoria)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IInventorRepository)Repository).SaveInventor(model, auditoria);
                return result;
            });
        }

        public ResultInfo DeleteInventor(int inventorId, Auditoria auditoria)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IInventorRepository)Repository).DeleteInventor(inventorId, auditoria);
                return result;
            });
        }


    }

}
