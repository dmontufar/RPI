using PI.Common;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IEstatusRepository : IRepository<Estatus> { }

    public interface IEventoRepository : IRepository<Eventos> { }

    public interface INizaRepository : IRepository<ClassificacionDeNiza> { }

    public interface IPaisRepository : IRepository<Pais> { }

    //public interface IReglaRepository : IRepository<Regla> { }

    public interface IVienaRepository : IRepository<ClasificacionDeViena> { }

    public interface IOpcionesPorEventoRepository : IRepository<OpcionesPorEvento> { }

    public interface ILeyesRepository : IRepository<Leyes> { }

    public interface IPermisoRepository : IRepository<Permiso> 
    {
        ResultInfo SaveAll(List<Permiso> model);
    }

    public interface IRolRepository : IRepository<Rol> { }

    public interface ISessionRepository : IRepository<Session> { }

    public interface IPublicacionRepository : IRepository<Publicacion> { }

    public interface ICronologiaRepository : IRepository<Cronologia> { }

    public interface IFavoritoRepository : IRepository<Favorito> {        
        PagedList GetPage(int pageNumber, int pageSize, int? idGrupo, int idUsuario);
        Favorito Add(Favorito entity, int grupoId);
    }

    public interface IGrupoRepository : IRepository<Grupo> { }

    public interface IGrupoExpedienteRepository : IRepository<GrupoExpediente> { }

    public interface IGrupoMiembroRepository : IRepository<GrupoMiembro> { }

    public interface IPatTitularRepository : IRepository<TitularEnPatentes> 
    {
        PagedList searchTitular(string textToSearch);
    }

    public interface IAgenteRepository : IRepository<Agente>
    {
        PagedList searchAgente(string textToSearch);
    }

    public interface IInventorRepository : IRepository<Inventor> 
    {
        ResultInfo SaveInventor(GenericEntity<Inventor> model, Auditoria auditoria);
        ResultInfo DeleteInventor(int inventorId, Auditoria auditoria);
    }
    
}
