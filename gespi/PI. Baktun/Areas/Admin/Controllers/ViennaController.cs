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
    public class ViennaController : CoreController
    {
        private IVienaManager _vienaManager;

        public ViennaController(IVienaManager vienaManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _vienaManager = vienaManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new ClasificacionDeViena() { Id = -1 };
                }
                else
                {
                    result = _vienaManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(ClasificacionDeViena model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _vienaManager.Create(model);
            }
            else
            {
                result = _vienaManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        //pageNumber == 0 >> all records
        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _vienaManager.GetPage(page, pageSize, null, order => order.Descripcion);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _vienaManager.GetPage(page, pageSize, f => f.Descripcion.Contains(filter), order => order.Descripcion);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
