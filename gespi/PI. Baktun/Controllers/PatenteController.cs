using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using PI.Models.Enums;
using PI.Baktun.core;

namespace PI.Baktun.Controllers
{
    public class PatenteController : CoreController
    {
        private IExpedienteManager _expedienteManager;
        private IPatenteManager _patenteManager;
        private IOpcionManager _opcionManager;

        public PatenteController(IExpedienteManager expedienteManager, IPatenteManager patenteManager, IOpcionManager opcionManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _expedienteManager = expedienteManager;
            _opcionManager = opcionManager;
            _patenteManager = patenteManager;
        }

        public JsonResult TestEndPoint()
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
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
                    var solicitud = _expedienteManager.GetExpedienteDePatentes(TipoDeRegistroId, Numero);

                    //var opciones = _opcionManager.GetOpcionesPorEstatus(solicitud.Expediente.EstatusId);

                    result.Result = new { documento = solicitud };
                }
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult Registro(int registro, int tipoDeRegistroId = 0)
        {
            if (IsAuth)
            {
                if (registro>0)
                {
                    var solicitud = _expedienteManager.GetExpedienteDePatentesPorRegistro(tipoDeRegistroId, registro);

                    result.Result = new { documento = solicitud };
                }
            }

            return new JsonNetResult() { Data = result };
        }

        // GET: /Test/1 >userid=1
        public JsonNetResult ExpedienteId(int id)
        {
            if (IsAuth)
            {
                if (id == -1)
                {
                    result.Succeeded = true;
                    var expediente = ExpedienteDePatente.CreateExpediente();
                    expediente.Expediente.Id = -1;
                    result.Result = expediente;
                }
                else
                {
                    // should get by id :/
                    var solicitud = _expedienteManager.GetExpedienteDePatentesPorId(id);

                    result.Result = new { documento = solicitud };
                }
            }

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult BusquedaPatentesDsc(int pageNumber, int pageSize, string textToSearch, int? tipoDeRegistro)
        {
            if (IsAuth)
            {
                result = _expedienteManager.BusquedaPatentesDsc(pageNumber, pageSize, textToSearch, tipoDeRegistro);
            }

            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        public JsonResult SaveSolicitud(ExpedienteDePatente model)
        {
            if (IsAuth)
            {
                if (model.Expediente.Id == -1) {
                    var cronologia = new ModelCronologia(); // footprint separate table
                    cronologia.Fecha = model.Expediente.FechaDeSolicitud;
                    model.Expediente.EstatusId = cronologia.EstatusId = (int)PatenteEstatus.Solicitud_Ingresada;
                    cronologia.Referencia = "";
                    cronologia.UsuarioId = sessionToken.UsuarioId;
                    model.Cronologia = new List<ModelCronologia>();
                    model.Cronologia.Add(cronologia);
                }
                model.Expediente.ModuloId = 2;

                result = _patenteManager.SaveSolicitud(model);


                //What when the solicitud is new???? how do we fetch it!!
                var solicitud = _expedienteManager.GetExpedienteDePatentesPorId(model.Expediente.Id);

                result.Result = new
                {
                    documento = solicitud,
                };

            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveTitulares(GenericEntity<TitularEnPatentes> model)
        {
            if (IsAuth)
            {
                var titular = model.Generic as TitularEnPatentes; //nombre, pais
                model.Extra = JObject.Parse(model.jsExtra);

                var patenteTitular = new TitularDeLaPatente()
                {
                    ExpedienteId = model.Extra.ExpedienteId,
                    Direccion = model.Extra.Direccion,
                    PaisId = titular.PaisId,
                    TitularId = titular.Id
                };
                var auditoria = new Auditoria() 
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Agregó Titular",
                    ExpedienteId = patenteTitular.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = model.Auditoria
                };

                model.Extra = patenteTitular;

                result = _patenteManager.SaveTitulares(model, auditoria);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult DeleteTitular(int titularId, int expedienteId, string historial)
        {
            if (IsAuth)
            {
                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Elimino Titular - " + titularId,
                    ExpedienteId = expedienteId,
                    Fecha = DateTime.Now,
                    Historial = historial
                };

                result = _patenteManager.DeleteTitular(titularId, auditoria);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveAgente(GenericEntity<Auditoria> model)
        {
            if (IsAuth)
            {
                model.Extra = JObject.Parse(model.jsExtra); // parsing additional data
                var historial = "AgenteId:"+model.Extra.AgenteId.ToString();
                
                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Grabó Agente - " + model.Extra.AgenteId,
                    ExpedienteId = model.Extra.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = historial
                };
                model.Generic = auditoria;

                result = _patenteManager.SaveAgente(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveResumenClasificacion(GenericEntity<Auditoria> model)
        {
            if (IsAuth)
            {
                model.Extra = JObject.Parse(model.jsExtra);
                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Grabó Agente - " + model.Extra.AgenteId,
                    ExpedienteId = model.Extra.ExpedienteId,
                    Fecha = DateTime.Now
                };
                model.Generic = auditoria; //Push historial on the repository

                result = _patenteManager.SaveResumenClasificacion(model);

            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        /// 
        /// REFERENCIA - PRIORIDAD
        /// 
        [HttpPost]
        public JsonResult SaveReferencia(GenericEntity<Prioridad> model)
        {
            if (IsAuth)
            {
                var prioridad = model.Generic as Prioridad; //prioridad data

                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Agregó prioridad",
                    ExpedienteId = prioridad.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = model.Auditoria
                };

                model.Extra = auditoria;

                result = _patenteManager.SaveReferencia(model);
            }
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        public JsonResult DeleteReferencia(GenericEntity<Prioridad> model)
        {
            if (IsAuth)
            {
                var prioridad = model.Generic as Prioridad; //prioridad data
                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Eliminó prioridad",
                    ExpedienteId = prioridad.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = model.Auditoria
                };

                model.Extra = auditoria;

                result = _patenteManager.DeleteReferencia(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        /// 
        /// ANUALIDADES
        /// 
        [HttpPost]
        public JsonResult SaveAnualidad(GenericEntity<Anualidad> model)
        {
            if (IsAuth)
            {
                var prioridad = model.Generic as Anualidad; //Anualidad data

                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Agregó Anualidad",
                    ExpedienteId = prioridad.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = model.Auditoria
                };

                model.Extra = auditoria;

                result = _patenteManager.SaveAnualidad(model);
            }
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        public JsonResult DeleteAnualidad(GenericEntity<Anualidad> model)
        {
            if (IsAuth)
            {
                var prioridad = model.Generic as Anualidad; //Anualidad data
                var auditoria = new Auditoria()
                {
                    UsuarioId = sessionToken.UsuarioId,
                    Evento = "Eliminó Anualidad",
                    ExpedienteId = prioridad.ExpedienteId,
                    Fecha = DateTime.Now,
                    Historial = model.Auditoria
                };

                model.Extra = auditoria;

                result = _patenteManager.DeleteAnualidad(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public ActionResult CambiarEstatus(ResolucionCustomizada tramite)
        {
            if (IsAuth)
            {
                var solicitud = _expedienteManager.GetExpedienteDePatentesPorId(tramite.ExpedienteId);
                tramite.DOCUMENT = JObject.Parse(tramite.JSONDOC);

                tramite.UsuarioId = sessionToken.UsuarioId;
                tramite.EstatusId = (int)tramite.EstatusId;
                tramite.EstatusFinalId = (int)tramite.EstatusId;
                tramite.UpdatesEstatus = true;
                tramite.Observaciones = "Cambio de Estatus";

                ViewBag.Solicitud = solicitud;
                ViewBag.Tramite = tramite;

                //PDF o HTML?
                var htmlString = "Cambio de Estatus";
                //tramite.HTMLDOC = htmlString;
                htmlString = tramite.HTMLDOC; // DEMO
                var cronologiaResult = _expedienteManager.SaveEventoCronologicoDePatentes(tramite);                

                return Content(htmlString, "text/html");
            }

            throw new HttpException(401, "");//Not Authorized
        }

        [HttpPost]
        public ActionResult ResolucionCustomizada(ResolucionCustomizada tramite)
        {
            if (IsAuth)
            {
                var solicitud = _expedienteManager.GetExpedienteDePatentesPorId(tramite.ExpedienteId);
                tramite.DOCUMENT = JObject.Parse(tramite.JSONDOC);

                tramite.UsuarioId = sessionToken.UsuarioId;
                tramite.EstatusId = (int)tramite.EstatusId;
                tramite.EstatusFinalId = (int)tramite.EstatusId;
                tramite.UpdatesEstatus = true;
                tramite.Observaciones = "Cambio de Estatus";

                ViewBag.Solicitud = solicitud;
                ViewBag.Tramite = tramite;

                var htmlString = RenderRazorViewToString("ResolucionCustomizada", null);
                tramite.HTMLDOC = htmlString;

                var cronologiaResult = _expedienteManager.SaveEventoCronologicoDePatentes(tramite);

                return Content(htmlString, "text/html");
            }

            throw new HttpException(401, "");//Not Authorized
        }

        private ResolucionCustomizada SetEstatusDeNotificacion(ResolucionCustomizada tramite) 
        {
            var exp = _expedienteManager.GetBaseDeExpedientePorId(tramite.ExpedienteId);
            switch (exp.EstatusId) 
            { 
                case (int)PatenteEstatus.Requerimiento_de_forma_pendiente_de_notificar:
                    tramite.EstatusId = (int)PatenteEstatus.Requerimiento_Forma_Notificado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Requerimiento_Forma_Notificado;
                    break;
                case (int)PatenteEstatus.Edicto_Emitido_Pendiente_De_Entregar:
                    tramite.EstatusId = (int)PatenteEstatus.Edicto_Notificado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Edicto_Notificado;
                    break;
                case (int)PatenteEstatus.Publicada:
                    tramite.EstatusId = (int)PatenteEstatus.Publicada;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Publicada;
                    tramite.Observaciones = "Vencimiento Plazo por Observaciones:" + tramite.Fecha.ToShortDateString() + ", Fecha en que se notificó " + tramite.Fecha.ToShortDateString();
                    break;
                case (int)PatenteEstatus.Orden_De_Pago_Pend_De_Notificacion:
                    tramite.EstatusId = (int)PatenteEstatus.Orden_De_Pago_Notificada;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Orden_De_Pago_Notificada;
                    break;
                case (int)PatenteEstatus.Requerimiento_Examen_de_Fondo:
                    tramite.EstatusId = (int)PatenteEstatus.Requerimiento_De_Examen_De_Fondo_Notificado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Requerimiento_De_Examen_De_Fondo_Notificado;
                    break;
                case (int)PatenteEstatus.Admision_a_Tramite:
                    tramite.EstatusId = (int)PatenteEstatus.Admision_Notificada;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Admision_Notificada;
                    break;
                case (int)PatenteEstatus.Resolucion_De_Consecion:
                    tramite.EstatusId = (int)PatenteEstatus.Resolucion_De_Consecion_NOTIFICADA;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Resolucion_De_Consecion_NOTIFICADA;
                    break;
                case (int)PatenteEstatus.Resolucion_De_Rechazo_Total:
                    tramite.EstatusId = (int)PatenteEstatus.Resolucion_De_Rechazo_Total_Notificado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Resolucion_De_Rechazo_Total_Notificado;
                    break;
                case (int)PatenteEstatus.Resolucion_De_Rechazo_Parcial:
                    tramite.EstatusId = (int)PatenteEstatus.Resolucion_De_Rechazo_Parcial_NOTIFICADA;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Resolucion_De_Rechazo_Parcial_NOTIFICADA;
                    break;
                default: //ERROR
                    tramite.EstatusId = (int)PatenteEstatus.Solicitud_Denegada;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Solicitud_Denegada;
                    tramite.Observaciones = "ERROR DEL SISTEMA";
                    break;
            }

            //IF thisform.optiongroup1.option1.value= 1 then
            //    replace mainpat.idstatus WITH "005"	
            //ENDIF
	
            //IF thisform.optiongroup1.option2.value= 1 then
            //    replace mainpat.idstatus WITH "006"	
            //endif

            return tramite;
        }

        private ActionResult resolucion(string item, ResolucionCustomizada tramite) 
        {

            var solicitud = _expedienteManager.GetExpedienteDePatentesPorId(tramite.ExpedienteId);
            tramite.DOCUMENT = JObject.Parse(tramite.JSONDOC);
            tramite = StampResolucion(tramite);

            switch (item)
            {
                case "PAT001": //Requerimiento_De_Examen_De_Forma
                    tramite.EstatusId = (int)PatenteEstatus.Examen_Tecnico_de_Forma_Efectuado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Requerimiento_de_forma_pendiente_de_notificar;
                    break;
                case "PAT002": //Gestor_Oficioso
                    tramite.EstatusId = (int)PatenteEstatus.Requerimiento_de_forma_pendiente_de_notificar;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Requerimiento_de_forma_pendiente_de_notificar;
                    break;
                case "PAT003": //Admision_Tramite
                    tramite.EstatusId = (int)PatenteEstatus.Admision_a_Tramite;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Admision_a_Tramite;
                    break;
                case "PAT004": //Edicto
                    tramite.EstatusId = (int)PatenteEstatus.Edicto_Emitido_Pendiente_De_Entregar;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Edicto_Emitido_Pendiente_De_Entregar;
                    break;
                case "PAT005": //Publicaciones
                    tramite.EstatusId = (int)PatenteEstatus.Publicada;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Publicada;
                    break;
                case "PAT006": //Admite_Observaciones
                    tramite.EstatusId = (int)PatenteEstatus.Publicada;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Publicada;
                    break;
                case "PAT007": //Orden_De_Pago
                    tramite.EstatusId = (int)PatenteEstatus.Orden_De_Pago_Pend_De_Notificacion;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Orden_De_Pago_Pend_De_Notificacion;
                    break;
                case "PAT008": //Pago_Examen
                    tramite.EstatusId = (int)PatenteEstatus.Pago_Examen;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Pago_Examen;
                    break;
                case "PAT009": //Requerimiento_Examen_De_Fondo_A
                    tramite.EstatusId = (int)PatenteEstatus.Requerimiento_Examen_de_Fondo;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Requerimiento_Examen_de_Fondo;
                    break;
                case "PAT010": //Requerimiento_Examen_de_Fondo_B
                    tramite.EstatusId = (int)PatenteEstatus.Requerimiento_Examen_de_Fondo;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Requerimiento_Examen_de_Fondo;
                    break;
                case "PAT011": //Reporte_De_Examen_De_Fondo
                    tramite.EstatusId = (int)PatenteEstatus.NO_CAMBIA;
                    tramite.EstatusFinalId = (int)PatenteEstatus.NO_CAMBIA;
                    tramite.Observaciones = "Reporte_De_Examen_De_Fondo";
                    break;
                case "PAT012": //Informe_de_Busqueda
                    tramite.EstatusId = (int)PatenteEstatus.NO_CAMBIA;
                    tramite.EstatusFinalId = (int)PatenteEstatus.NO_CAMBIA;
                    tramite.Observaciones = "Informe_de_Busqueda";
                    break;
                case "PAT013": //Razon_De_Abandono
                    tramite.EstatusId = (int)PatenteEstatus.Abandonado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Abandonado;
                    tramite.Observaciones = "Razon_De_Abandono";
                    break;
                case "PAT014": //Desistimiento_Solicitud
                    tramite.EstatusId = (int)PatenteEstatus.Desestimiento_Ley_57_2000;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Desestimiento_Ley_57_2000;
                    break;
                case "PAT015": //Titulo
                    tramite.EstatusId = (int)PatenteEstatus.Registro_Efectuado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Registro_Efectuado;
                    tramite.Registrar = true;
                    break;
                case "PAT016": //Notificacion
                    tramite = SetEstatusDeNotificacion(tramite);
                    break;
                case "PAT017": //Traspaso
                    // Esta opcion cambia el titular y la direccion
                    break;
                case "PAT018": //Cambio_De_Nombre
                    tramite.EstatusId = (int)PatenteEstatus.TRASPASO_DE_PATENTE;
                    tramite.EstatusFinalId = (int)PatenteEstatus.TRASPASO_DE_PATENTE;
                    break;
                case "PAT019": //Titulo_Renovacion
                    tramite.EstatusId = (int)PatenteEstatus.Registro_Efectuado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Registro_Efectuado;
                    break;
                case "PAT020": //Reposicion_Titulo_Patente
                    tramite.EstatusId = (int)PatenteEstatus.Registro_Efectuado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Registro_Efectuado;
                    break;
                case "PAT021": //Certificaciones
                    tramite.EstatusId = (int)PatenteEstatus.Registro_Efectuado;
                    tramite.EstatusFinalId = (int)PatenteEstatus.Registro_Efectuado;
                    break;
            }
            tramite.UpdatesEstatus = true;

            ViewBag.Solicitud = solicitud;
            ViewBag.Tramite = tramite;

            var htmlString = String.Empty;
            switch (item) 
            {
                case "PAT001": //Requerimiento_De_Examen_De_Forma
                    htmlString = RenderRazorViewToString("Requerimiento_De_Examen_De_Forma", null);
                    break;
                case "PAT002": //Gestor_Oficioso
                    htmlString = RenderRazorViewToString("Gestor_Oficioso", null);
                    break;
                case "PAT003": //Admision_Tramite
                    htmlString = RenderRazorViewToString("Admision_Tramite", null);
                    break;
                case "PAT004": //Edicto
                    htmlString = RenderRazorViewToString("Edicto", null);
                    break;
                case "PAT005": //Publicaciones
                    htmlString = RenderRazorViewToString("Publicaciones", null);
                    break;
                case "PAT006": //Admite_Observaciones
                    htmlString = RenderRazorViewToString("Admite_Observaciones", null);
                    break;
                case "PAT007": //Orden_De_Pago
                    htmlString = RenderRazorViewToString("Orden_De_Pago", null);
                    break;
                case "PAT008": //Pago_Examen
                    htmlString = RenderRazorViewToString("Pago_Examen", null);
                    break;
                case "PAT009": //Requerimiento_Examen_De_Fondo_A
                    htmlString = RenderRazorViewToString("Requerimiento_Examen_De_Fondo_A", null);
                    break;
                case "PAT010": //Requerimiento_Examen_de_Fondo_B
                    htmlString = RenderRazorViewToString("Requerimiento_Examen_de_Fondo_B", null);
                    break;
                case "PAT011": //Reporte_De_Examen_De_Fondo
                    htmlString = RenderRazorViewToString("Reporte_De_Examen_De_Fondo", null);
                    break;
                case "PAT012": //Informe_de_Busqueda
                    htmlString = RenderRazorViewToString("Informe_de_Busqueda", null);
                    break;
                case "PAT013": //Razon_De_Abandono
                    htmlString = RenderRazorViewToString("Razon_De_Abandono", null);
                    break;
                case "PAT014": //Desistimiento_Solicitud
                    htmlString = RenderRazorViewToString("Desistimiento_Solicitud", null);
                    break;
                case "PAT015": //Titulo
                    htmlString = RenderRazorViewToString("Titulo", null);
                    break;
                case "PAT016": //Notificacion
                    htmlString = RenderRazorViewToString("Notificacion", null);
                    break;
                case "PAT017": //Traspaso
                    htmlString = RenderRazorViewToString("Traspaso", null);
                    break;
                case "PAT018": //Cambio_De_Nombre
                    htmlString = RenderRazorViewToString("Cambio_De_Nombre", null);
                    break;
                case "PAT019": //Titulo_Renovacion
                    htmlString = RenderRazorViewToString("Titulo_Renovacion", null);
                    break;
                case "PAT020": //Reposicion_Titulo_Patente
                    htmlString = RenderRazorViewToString("Reposicion_Titulo_Patente", null);
                    break;
                case "PAT021": //Certificaciones
                    htmlString = RenderRazorViewToString("Certificaciones", null);
                    break;
            }

            tramite.HTMLDOC = htmlString;
            var cronologiaResult = _expedienteManager.SaveEventoCronologicoDePatentes(tramite);

            // save again and update res...
            return Content(htmlString, "text/html");

        }


        [HttpPost]
        [CheckSession(item = "PAT001")]
        public ActionResult Requerimiento_De_Examen_De_Forma(ResolucionCustomizada tramite)
        {

            return resolucion("PAT001", tramite);
        }


        [HttpPost]
        [CheckSession(item = "PAT002")]
        public ActionResult Gestor_Oficioso(ResolucionCustomizada tramite)
        {

            return resolucion("PAT002", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT003")]
        public ActionResult Admision_Tramite(ResolucionCustomizada tramite)
        {

            return resolucion("PAT003", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT004")]
        public ActionResult Edicto(ResolucionCustomizada tramite)
        {

            return resolucion("PAT004", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT005")]
        public ActionResult Publicaciones(ResolucionCustomizada tramite)
        {

            return resolucion("PAT005", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT006")]
        public ActionResult Admite_Observaciones(ResolucionCustomizada tramite)
        {

            return resolucion("PAT006", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT007")]
        public ActionResult Orden_De_Pago(ResolucionCustomizada tramite)
        {

            return resolucion("PAT007", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT008")]
        public ActionResult Pago_Examen(ResolucionCustomizada tramite)
        {

            return resolucion("PAT008", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT009")]
        public ActionResult Requerimiento_Examen_De_Fondo_A(ResolucionCustomizada tramite)
        {

            return resolucion("PAT009", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT010")]
        public ActionResult Requerimiento_Examen_de_Fondo_B(ResolucionCustomizada tramite)
        {

            return resolucion("PAT010", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT011")]
        public ActionResult Reporte_De_Examen_De_Fondo(ResolucionCustomizada tramite)
        {

            return resolucion("PAT011", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT012")]
        public ActionResult Informe_de_Busqueda(ResolucionCustomizada tramite)
        {

            return resolucion("PAT012", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT013")]
        public ActionResult Razon_De_Abandono(ResolucionCustomizada tramite)
        {

            return resolucion("PAT013", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT014")]
        public ActionResult Desistimiento_Solicitud(ResolucionCustomizada tramite)
        {

            return resolucion("PAT014", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT015")]
        public ActionResult Titulo(ResolucionCustomizada tramite)
        {

            return resolucion("PAT015", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT016")]
        public ActionResult Notificacion(ResolucionCustomizada tramite)
        {

            return resolucion("PAT016", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT017")]
        public ActionResult Traspaso(ResolucionCustomizada tramite)
        {

            return resolucion("PAT017", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT018")]
        public ActionResult Cambio_De_Nombre(ResolucionCustomizada tramite)
        {

            return resolucion("PAT018", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT019")]
        public ActionResult Titulo_Renovacion(ResolucionCustomizada tramite)
        {

            return resolucion("PAT019", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT020")]
        public ActionResult Reposicion_Titulo_Patente(ResolucionCustomizada tramite)
        {

            return resolucion("PAT020", tramite);
        }

        [HttpPost]
        [CheckSession(item = "PAT021")]
        public ActionResult Certificaciones(ResolucionCustomizada tramite)
        {

            return resolucion("PAT021", tramite);
        }


        /*
         Requerimiento_De_Examen_De_Forma
        Gestor_Oficioso
        Admision_Tramite
        Edicto
        Publicaciones
        Admite_Observaciones
        Orden_De_Pago
        Pago_Examen
        Requerimiento_Examen_De_Fondo_A
        Requerimiento_Examen_de_Fondo_B
        Reporte_De_Examen_De_Fondo
        Informe_de_Busqueda
        Razon_De_Abandono
        Desistimiento_Solicitud
        Titulo
        Notificacion
        Traspaso
        Cambio_De_Nombre
        Titulo_Renovacion
        Reposicion_Titulo_Patente
        Certificaciones
         */

    }

}
