using Newtonsoft.Json.Linq;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class InventoresController : CoreController
    {
        private IInventorManager _inventorManager;

        public InventoresController(IInventorManager inventorManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _inventorManager = inventorManager;
        }


        [HttpPost]
        public JsonResult SaveInventor(GenericEntity<Inventor> model)
        {
            if (IsAuth)
            {
                var inventor = model.Generic as Inventor; //inventor data

                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Agregó Inventor",
                    ExpedienteId = inventor.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = model.Auditoria
                };

                model.Extra = inventor;

                result = _inventorManager.SaveInventor(model, auditoria);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteInventor(int inventorId, int expedienteId, string historial)
        {
            if (IsAuth)
            {
                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Eliminó Agente - " + inventorId,
                    ExpedienteId = expedienteId,
                    Fecha = DateTime.Now,
                    Historial = historial
                };

                result = _inventorManager.DeleteInventor(inventorId, auditoria);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }



    }
}
