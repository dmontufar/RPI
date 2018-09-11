using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    /// <summary>
    /// Working site eRPI
    /// Accesso a la administracion de expedientes de marcas, patentes y derecho de autor y sub-modulos de marcas
    /// </summary>
    public class eRPIController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        } 
	}
}