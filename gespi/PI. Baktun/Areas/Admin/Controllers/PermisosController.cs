using Newtonsoft.Json;
using PI.Baktun.Controllers;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Areas.Admin.Controllers
{
    public class PermisosController : CoreController
    {

        private IPermisoManager _permisoManager;

        public PermisosController(IPermisoManager permisoManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _permisoManager = permisoManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Agente() { Id = -1 };
                }
                else
                {
                    result = _permisoManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(List<Permiso> model)
        {
            if (IsAuth)
            {
                // assign user that updates
                model[0].OtorgadoPorUsuarioId = sessionToken.UsuarioId;
                model[1].OtorgadoPorUsuarioId = sessionToken.UsuarioId;
                result = _permisoManager.SaveAll(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetAll(int usuarioId=0)
        {
            if (IsAuth)
                result = _permisoManager.GetPage(1, 1000, u => u.UsuarioId == usuarioId, order => order.Opcion);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
