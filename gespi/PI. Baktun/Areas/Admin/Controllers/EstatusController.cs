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
    public class EstatusController : CoreController
    {
        private IEstatusManager _estatusManager;
        
        public EstatusController(IEstatusManager estatusManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _estatusManager = estatusManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Estatus() { Id = -1 };
                }
                else
                {
                    result = _estatusManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Estatus model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _estatusManager.Create(model);
            }
            else
            {
                result = _estatusManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10, int moduloId = 1)
        {
            if (IsAuth)
                result = _estatusManager.GetPage(page, pageSize, f => f.ModuloId == moduloId, order => order.Codigo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0, int moduloId=1)
        {
            if (IsAuth)
            {
                if (string.IsNullOrEmpty(filter))
                {
                    result = _estatusManager.GetPage(page, pageSize, f => f.ModuloId == moduloId, order => order.Codigo);
                }
                else
                {
                    result = _estatusManager.GetPage(page, pageSize, f => f.Descripcion.Contains(filter) && f.ModuloId == moduloId, order => order.Codigo);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
