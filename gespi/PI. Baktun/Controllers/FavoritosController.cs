using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class FavoritosController : CoreController
    {
        private IFavoritoManager _favoritoManager;
        private IGrupoManager _grupoManager;
        private IGrupoExpedienteManager _grupoExpedienteManager;
        private IGrupoMiembroManager _grupoMiembroManager;

        public FavoritosController(IFavoritoManager favoritoManager, IGrupoManager grupoManager, IGrupoExpedienteManager grupoExpedienteManager, IGrupoMiembroManager grupoMiembroManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _favoritoManager = favoritoManager;
            _grupoManager = grupoManager;
            _grupoExpedienteManager = grupoExpedienteManager;
            _grupoMiembroManager = grupoMiembroManager;
        }

        // GET: /Test/1 >userid=1
        public JsonNetResult AddGrupo(Grupo grupo)
        {
            if (IsAuth)
            {

                var ownerId = 1; // fake !!
                var glist = _grupoManager.GetPage(0, 0, e => e.OwnerId == ownerId, g => g.Nombre);

                if (((PagedList)glist.Result).TotalItems < 10)
                {
                    grupo.OwnerId = 1; // fake !!
                    grupo.CreatedDate = DateTime.Now;
                    result = _grupoManager.Create(grupo);
                }
                else
                {
                    result.Succeeded = false;
                    result.Result = "10 grupos max!";
                }
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult UpdateGrupo(Grupo grupo)
        {
            if (IsAuth)
            {
                grupo.OwnerId = 1; // fake !!
                grupo.CreatedDate = DateTime.Now;
                result = _grupoManager.Create(grupo);
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult GetGrupos()
        {
            if (IsAuth)
            {
                var ownerId = 1; // fake !!
                result = _grupoManager.GetPage(0, 0, e => e.OwnerId == ownerId, g => g.Nombre);
            }

            return new JsonNetResult() { Data = result };
        }


        [HttpPost]
        public JsonNetResult AddFavorito(int expediente, int grupoId)
        {
            if (IsAuth)
            {
                var favorito = new Favorito()
                {                    
                    ExpedienteId = expediente,                    
                    UsuarioId = 1 // Fake!!
                };

                // al agregar el primer favorito tendria que crear un grupo por default

                // verificar que expediente no existe para el usuario, si existe des-agregarlo;
                result = _favoritoManager.Create(favorito, grupoId);
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult UpdateFavorito(Favorito favorito)
        {
            if (IsAuth)
            {
                //trust Id y notas
                favorito.UsuarioId = 1; // Fake!!                

                // verificar que expediente no existe para el usuario, si existe des-agregarlo;
                result = _favoritoManager.Update(favorito);
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult GetFavoritosPageFilter(int idGrupoFilter = 0, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
            {
                var usuarioId = sessionToken.UsuarioId; // fake!!
                if (idGrupoFilter==0)
                {
                    result = _favoritoManager.GetPage(page, pageSize, null, usuarioId);
                }
                else
                {
                    result = result = _favoritoManager.GetPage(page, pageSize, idGrupoFilter, usuarioId);
                }
            }

            return new JsonNetResult() { Data = result };
        }



    }
}
