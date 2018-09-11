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
    public class RolesController : CoreController
    {
        private IRolManager _rolManager;

        public RolesController(IRolManager rolManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _rolManager = rolManager;
        }
        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new Rol() { Id = -1 };
                }
                else
                {
                    result = _rolManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Rol model)
        {
            var result = new ResultInfo();
            if (model.Id == -1)
            {
                result = _rolManager.Create(model);
            }
            else
            {
                result = _rolManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _rolManager.GetPage(page, pageSize, null, order => order.Nombre);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _rolManager.GetPage(page, pageSize, f => f.Nombre.Contains(filter), order => order.Nombre);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
