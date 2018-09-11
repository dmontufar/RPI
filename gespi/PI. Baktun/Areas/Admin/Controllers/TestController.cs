using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        public JsonResult Test()
        {
            return Json("(.)(.)", JsonRequestBehavior.AllowGet);
        }
	}
}