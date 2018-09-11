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
    public class PaisesController : CoreController
    {
        private IPaisManager _paisManager;

        public PaisesController(IPaisManager paisManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _paisManager = paisManager;
        }
        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Pais() { Id = -1 };
                }
                else
                {
                    result = _paisManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Pais model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _paisManager.Create(model);
            }
            else
            {
                result = _paisManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _paisManager.GetPage(page, pageSize, null, order => order.Nombre);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _paisManager.GetPage(page, pageSize, f => f.Nombre.Contains(filter), order => order.Nombre);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
