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
    public class OpcionesController : CoreController
    {
        private IOpcionManager _opcionManager;

        public OpcionesController(IOpcionManager opcionManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _opcionManager = opcionManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Opciones() { Id = -1 };
                }
                else
                {
                    result = _opcionManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Opciones model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _opcionManager.Create(model);
            }
            else
            {
                result = _opcionManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10, int moduloId = 1)
        {
            if (IsAuth)
                result = _opcionManager.GetPage(page, pageSize, f => f.ModuloId == moduloId, order => order.Descripcion);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0, int moduloId=1)
        {
            if (IsAuth)
            {
                if (string.IsNullOrEmpty(filter))
                {
                    result = _opcionManager.GetPage(page, pageSize, f => f.ModuloId == moduloId, order => order.Descripcion);
                }
                else
                {
                    result = _opcionManager.GetPage(page, pageSize, f => f.Descripcion.Contains(filter) && f.ModuloId == moduloId, order => order.Descripcion);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
