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
    public class EventosController : CoreController
    {
        private IEventoManager _eventoManager;

        public EventosController(IEventoManager eventoManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _eventoManager = eventoManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Eventos() { Id = -1 };
                }
                else
                {
                    result = _eventoManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Eventos model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _eventoManager.Create(model);
            }
            else
            {
                result = _eventoManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10, int moduloId = 1)
        {
            if (IsAuth)
                result = _eventoManager.GetPage(page, pageSize, f => f.ModuloId == moduloId, order => order.Id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0, int moduloId=1)
        {
            if (IsAuth)
            {
                if (string.IsNullOrEmpty(filter))
                {
                    result = _eventoManager.GetPage(page, pageSize, f => f.ModuloId == moduloId, order => order.Id);
                }
                else
                {
                    result = _eventoManager.GetPage(page, pageSize, f => f.Descripcion.Contains(filter) && f.ModuloId == moduloId, order => order.Id);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
