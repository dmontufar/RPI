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
    public class OpsXEventoController : CoreController
    {
        private IOpcionesPorEventoManager _oxeManager;

        public OpsXEventoController(IOpcionesPorEventoManager oxeManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _oxeManager = oxeManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new OpcionesPorEvento() { Id = -1 };
                }
                else
                {
                    result = _oxeManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(OpcionesPorEvento model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _oxeManager.Create(model);
            }
            else
            {
                result = _oxeManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10, int eventoId=0)
        {
            if (IsAuth)
                result = _oxeManager.GetPage(page, pageSize, x => x.EventoId == eventoId, order => order.OpcionId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0, int eventoId = 0)
        {
            int op = int.Parse(filter);

            if (IsAuth)
                result = _oxeManager.GetPage(page, pageSize, x => x.EventoId == eventoId && x.OpcionId == op, order => order.OpcionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
