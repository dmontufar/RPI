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
    public class MarcaController : CoreController
    {
        private IExpedienteManager _expedienteManager;
        private IMarcaManager _marcaManager;
        private IOpcionManager _opcionManager;

        public MarcaController(IExpedienteManager expedienteManager, IMarcaManager marcaManager, IOpcionManager opcionManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _expedienteManager = expedienteManager;
            _marcaManager = marcaManager;
            _opcionManager = opcionManager;
        }

        // GET: /Test/1 >userid=1
        public JsonNetResult Expediente(string numero)
        {
            if (IsAuth)
            {
                if (string.IsNullOrWhiteSpace(numero))
                {
                    result.Succeeded = true;
                    var expediente = ExpedienteDeMarca.CreateExpediente();
                    expediente.Expediente.Id = -1;
                    result.Result = expediente;
                }
                else
                {
                    var solicitud = _expedienteManager.GetExpedienteDeMarcas(numero);
                    
                    result.Result = new {
                        documento = solicitud
                    };
                }
            }

            return new JsonNetResult() { Data = result };
        }

        // GET: /Test/1 >userid=1
        public JsonNetResult Registro(int tipoDeRegistroId, int registro, string letra)
        {
            if (IsAuth)
            {
                if (registro>0)
                {
                    var solicitud = _expedienteManager.GetExpedienteDeMarcasPorRegistro(tipoDeRegistroId, registro, letra);

                    result.Result = new
                    {
                        documento = solicitud
                    };
                }
                else
                {
                    result.Succeeded = false;
                }
            }

            return new JsonNetResult() { Data = result };
        }

        // GET: /Test/1 >userid=1
        public JsonNetResult ExpedienteId(int id)
        {
            if (IsAuth)
            {
                var solicitud = _expedienteManager.GetExpedienteDeMarcasPorId(id);

                result.Result = new
                {
                    documento = solicitud
                };
            }

            return new JsonNetResult() { Data = result };
        }


        public JsonNetResult BusquedaFonetica(int pageNumber, int pageSize, string textToSearch, string csvClases)
        {
            if (IsAuth)
            {
                result = _expedienteManager.BusquedaFonetica(pageNumber, pageSize, textToSearch, csvClases);
            }

            return new JsonNetResult() { Data = result };
        }


        public JsonNetResult BusquedaIdentica(int pageNumber, int pageSize, string textToSearch, string csvClases)
        {
            if (IsAuth)
            {
                result = _expedienteManager.BusquedaIdentica(pageNumber, pageSize, textToSearch, csvClases);
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult SearchTitular(string textToSearch)
        {
            if (IsAuth)
            {
                result = _marcaManager.searchTitular(textToSearch);
            }

            return new JsonNetResult() { Data = result };
        }



        public JsonResult Save(ExpedienteDeMarca model)
        {
            if (model.Expediente.Id == -1)
            {
                //result = _paisManager.Create(model);
            }
            else
            {
                //result = _paisManager.Update(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult Escaneo()
        {
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult EForma(string accion)
        {
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        //
        // GRABAR/IMPRIMIR EDICTO
        //
        [HttpPost]
        public ActionResult Edicto(ResolucionCustomizada tramite)
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

                var htmlString = RenderRazorViewToString("EdictoInterno", null);
                tramite.HTMLDOC = htmlString;

                //var cronologiaResult = _expedienteManager.SaveEventoCronologicoDeMarcas(tramite);

                return Content(htmlString, "text/html");
            }

            throw new HttpException(401, "");//Not Authorized
        }

        //
        // RESOLUCION CUSTOMIZADA
        //
        [HttpPost]
        [CheckSession(item = "MAR810")]
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

       
        private ResolucionCustomizada SetEstatusDeNotificacion(ResolucionCustomizada tramite)
        {

            var exp = _expedienteManager.GetBaseDeExpedientePorId(tramite.ExpedienteId);
            switch (exp.EstatusId)
            {
                case (int)MarcaEstatus.Estatus_Temporal:
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    //tramite.Observaciones = "Vencimiento Plazo por Observaciones:" + tramite.Fecha.ToShortDateString() + ", Fecha en que se notifico " + tramite.Fecha.ToShortDateString();
                    break;
                default: //ERROR
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.Observaciones = "ERROR DEL SISTEMA";
                    break;
            }
            return tramite;
        }

        private ActionResult resolucion(string item, ResolucionCustomizada tramite)
        {

            var solicitud = _expedienteManager.GetExpedienteDeMarcasPorId(tramite.ExpedienteId);
            tramite.DOCUMENT = JObject.Parse(tramite.JSONDOC);
            tramite = StampResolucion(tramite);

            switch (item)
            {
                case "MAR101": //Gestor Oficioso
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR102": //Requerimientos
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR103": //Objeciones Forma
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR201": //Objeciones Fondo
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR400": //Edictos
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR600": //Orden de Pago
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR700": //Titulo
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR801": //Reposicion de edicto
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR802": //Enmienda
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR803": //Cancelacion
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR804": //Traspaso
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR805": //Division Registro
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR806": //Abandono
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR807": //Desistimiento
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR808": //Rechazo por objecion
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR809": //Revocatoria de Oficio
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR810": //Resolucion Customizada
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR811": //Errores Materiales
                    tramite.EstatusId = (int)MarcaEstatus.Estatus_Temporal;
                    tramite.EstatusFinalId = (int)MarcaEstatus.Estatus_Temporal;
                    break;
                case "MAR900": //Cambiar Estatus
                    tramite.EstatusId = (int)tramite.EstatusId;
                    tramite.EstatusFinalId = (int)tramite.EstatusId;
                    tramite.Observaciones = "Cambio de Estatus";
                    break;
                case "MAR901": //Notificaciones
                    tramite = SetEstatusDeNotificacion(tramite);
                    break;
            }
            tramite.UpdatesEstatus = true;

            ViewBag.Solicitud = solicitud;
            ViewBag.Tramite = tramite;

            var htmlString = String.Empty;

            switch (item)
            {
                case "MAR101": //Gestor Oficioso
                    htmlString = RenderRazorViewToString("Gestor_Oficioso", null);
                    break;
                case "MAR102": //Requerimientos
                    htmlString = RenderRazorViewToString("Requerimientos", null);
                    break;
                case "MAR103": //Objeciones Forma
                    htmlString = RenderRazorViewToString("Objeciones_Forma", null);
                    break;
                case "MAR201": //Objeciones Fondo
                    htmlString = RenderRazorViewToString("Objeciones_Fondo", null);
                    break;
                case "MAR400": //Edictos
                    htmlString = RenderRazorViewToString("Edictos", null);
                    break;
                case "MAR600": //Orden de Pago
                    htmlString = RenderRazorViewToString("Orden_de_Pago", null);
                    break;
                case "MAR700": //Titulo
                    htmlString = RenderRazorViewToString("Titulo", null);
                    break;
                case "MAR801": //Reposicion de edicto
                    htmlString = RenderRazorViewToString("Reposicion_de_edicto", null);
                    break;
                case "MAR802": //Enmienda
                    htmlString = RenderRazorViewToString("Enmienda", null);
                    break;
                case "MAR803": //Cancelacion
                    htmlString = RenderRazorViewToString("Cancelacion", null);
                    break;
                case "MAR804": //Traspaso
                    htmlString = RenderRazorViewToString("Traspaso", null);
                    break;
                case "MAR805": //Division Registro
                    htmlString = RenderRazorViewToString("Division_Registro", null);
                    break;
                case "MAR806": //Abandono
                    htmlString = RenderRazorViewToString("Abandono", null);
                    break;
                case "MAR807": //Desistimiento
                    htmlString = RenderRazorViewToString("Desistimiento", null);
                    break;
                case "MAR808": //Rechazo por objecion
                    htmlString = RenderRazorViewToString("Rechazo_por_objecion", null);
                    break;
                case "MAR809": //Revocatoria de Oficio
                    htmlString = RenderRazorViewToString("Revocatoria_de_Oficio", null);
                    break;
                case "MAR810": //Resolucion Customizada
                    htmlString = RenderRazorViewToString("Resolucion_Customizada", null);
                    break;
                case "MAR811": //Errores Materiales
                    htmlString = RenderRazorViewToString("Errores_Materiales", null);
                    break;
                case "MAR900": //Cambiar Estatus
                    htmlString = "Cambio de Estatus";
                    break;
                case "MAR901": //Notificaciones
                    htmlString = RenderRazorViewToString("Notificaciones", null);
                    break;
            }

            tramite.HTMLDOC = htmlString;
            var cronologiaResult = _expedienteManager.SaveEventoCronologicoDeMarcas(tramite);

            // save again and update res...
            return Content(htmlString, "text/html");
        }

        [HttpPost]
        [CheckSession(item = "MAR101")]
        public ActionResult GestorOficioso(ResolucionCustomizada tramite)
        {
            return resolucion("MAR101", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR102")]
        public ActionResult Requerimientos(ResolucionCustomizada tramite)
        {
            return resolucion("MAR102", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR103")]
        public ActionResult ObjecionesForma(ResolucionCustomizada tramite)
        {
            return resolucion("MAR103", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR201")]
        public ActionResult ObjecionesFondo(ResolucionCustomizada tramite)
        {
            return resolucion("MAR201", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR400")]
        public ActionResult Edictos(ResolucionCustomizada tramite)
        {
            return resolucion("MAR400", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR600")]
        public ActionResult OrdendePago(ResolucionCustomizada tramite)
        {
            return resolucion("MAR600", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR700")]
        public ActionResult Titulo(ResolucionCustomizada tramite)
        {
            return resolucion("MAR700", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR801")]
        public ActionResult Reposiciondeedicto(ResolucionCustomizada tramite)
        {
            return resolucion("MAR801", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR802")]
        public ActionResult Enmienda(ResolucionCustomizada tramite)
        {
            return resolucion("MAR802", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR803")]
        public ActionResult Cancelacion(ResolucionCustomizada tramite)
        {
            return resolucion("MAR803", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR804")]
        public ActionResult Traspaso(ResolucionCustomizada tramite)
        {
            return resolucion("MAR804", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR805")]
        public ActionResult DivisionRegistro(ResolucionCustomizada tramite)
        {
            return resolucion("MAR805", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR806")]
        public ActionResult Abandono(ResolucionCustomizada tramite)
        {
            return resolucion("MAR806", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR807")]
        public ActionResult Desistimiento(ResolucionCustomizada tramite)
        {
            return resolucion("MAR807", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR808")]
        public ActionResult Rechazoporobjecion(ResolucionCustomizada tramite)
        {
            return resolucion("MAR808", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR809")]
        public ActionResult RevocatoriadeOficio(ResolucionCustomizada tramite)
        {
            return resolucion("MAR809", tramite);
        }


        [HttpPost]
        [CheckSession(item = "MAR811")]
        public ActionResult ErroresMateriales(ResolucionCustomizada tramite)
        {
            return resolucion("MAR811", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR900")]
        public ActionResult CambiarEstatus(ResolucionCustomizada tramite)
        {
            return resolucion("MAR900", tramite);
        }

        [HttpPost]
        [CheckSession(item = "MAR901")]
        public ActionResult Notificaciones(ResolucionCustomizada tramite)
        {
            return resolucion("MAR901", tramite);
        }
    }
}
