using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class AdminController : Controller
    {
        /// <summary>
        /// Modulo de GESPI Admin
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
	}
}