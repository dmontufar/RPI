using Newtonsoft.Json.Linq;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class ExpedienteController : CoreController
    {
        private IExpedienteManager _expedienteManager;
        private IMarcaManager _marcaManager;
        private IOpcionManager _opcionManager;

        public ExpedienteController(IExpedienteManager expedienteManager, IMarcaManager marcaManager, IOpcionManager opcionManager, ISessionManager sessionManager) : base(sessionManager)
        {
            _expedienteManager = expedienteManager;
            _marcaManager = marcaManager;
            _opcionManager = opcionManager;
        }


/*
 *
 * Este va en un controller generico
 * 
 */
        public ActionResult GetDOCResol(int cronologiaId)
        {
            if (IsAuth)
            {
                var htmlString = _expedienteManager.getDOCResol(cronologiaId);
                return Content(htmlString, "text/html");
            }

            throw new HttpException(401, "");//Not Authorized
        }

    }
}
