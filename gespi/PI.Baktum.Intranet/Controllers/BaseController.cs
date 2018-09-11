using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    /// <summary>
    /// eRegistro ROOT
    /// </summary>
    public class BaseController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
