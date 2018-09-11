using Newtonsoft.Json.Linq;
using PI.Baktun.core;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using PI.Models.Composite;
using PI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class AnotacionController : CoreController
    {
        private IExpedienteManager _expedienteManager;
        private IMarcaManager _marcaManager;
        private IOpcionManager _opcionManager;

        public AnotacionController(IExpedienteManager expedienteManager, IMarcaManager marcaManager, IOpcionManager opcionManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _expedienteManager = expedienteManager;
            _marcaManager = marcaManager;
            _opcionManager = opcionManager;
        }


        // GET: /Test/1 >userid=1
        public JsonResult Expediente(string Numero, int TipoDeRegistroId = 0)
        {
            if (IsAuth)
            {
                if (string.IsNullOrEmpty(Numero))
                {
                    result.Succeeded = true;
                    var expediente = ExpedienteDePatente.CreateExpediente();
                    expediente.Expediente.Id = -1;
                    result.Result = expediente;
                }
                else
                {
                    var solicitud = _expedienteManager.GetExpedienteDeAnotaciones(TipoDeRegistroId, Numero);

                    result.Result = new { documento = solicitud };
                }
            }

            return new JsonNetResult() { Data = result };
        }


        //
        // RESOLUCION CUSTOMIZADA
        //
        [HttpPost]
        //[CheckSession(item = "MAR810")]
        public ActionResult ResolucionCustomizada(ResolucionCustomizada tramite)
        {
            if (IsAuth)
            {
                var solicitud = _expedienteManager.GetExpedienteDeMarcasPorId(tramite.ExpedienteId);
                tramite.DOCUMENT = JObject.Parse(tramite.JSONDOC);

                tramite.UsuarioId = sessionToken.UsuarioId;
                tramite.EstatusId = (int)tramite.EstatusId;
                tramite.EstatusFinalId = (int)tramite.EstatusId;
                tramite.UpdatesEstatus = tramite.UpdatesEstatus;
                tramite.Observaciones = "Cambio de Estatus";

                ViewBag.Solicitud = solicitud;
                ViewBag.Tramite = tramite;

                var htmlString = RenderRazorViewToString("ResolucionCustomizada", null);
                tramite.HTMLDOC = htmlString;

                var cronologiaResult = _expedienteManager.SaveEventoCronologicoDeMarcas(tramite);

                return Content(htmlString, "text/html");
            }

            throw new HttpException(401, "");//Not Authorized
        }


    }
}
