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
    public class LeyesController : CoreController
    {

        private ILeyesManager _leyManager;

        public LeyesController(ILeyesManager leyManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _leyManager = leyManager;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Leyes() { Id = -1 };
                }
                else
                {
                    result = _leyManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Leyes model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _leyManager.Create(model);
            }
            else
            {
                result = _leyManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _leyManager.GetPage(page, pageSize, null, order => order.Ley);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _leyManager.GetPage(page, pageSize, f => f.Ley.Contains(filter), order => order.Ley);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
