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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class DAutorController : CoreController
    {
        private IExpedienteManager _expedienteManager;
        private IDerechoDeAutorManager _derechoDeAutorManager;
        private IOpcionManager _opcionManager;

        public DAutorController(IExpedienteManager expedienteManager, IDerechoDeAutorManager derechoDeAutorManager, IMarcaManager marcaManager, IOpcionManager opcionManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _expedienteManager = expedienteManager;
            _opcionManager = opcionManager;
            _derechoDeAutorManager = derechoDeAutorManager;
        }

        // GET: /Test/1 >userid=1
        [CheckSession(item = "n/a")]
        public JsonResult Expediente(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                result.Succeeded = true;
                var expediente = ExpedienteDAutor.CreateExpediente();
                expediente.Expediente.Id = -1;
                result.Result = expediente;
            }
            else
            {
                var solicitud = _expedienteManager.GetExpedienteDerechoAutor(numero);

                //var opciones = _opcionManager.GetOpcionesPorEstatus(solicitud.Expediente.EstatusId);

                result.Result = new
                {
                    documento = solicitud
                };
            }

            //return Json(result, JsonRequestBehavior.AllowGet);
            return new JsonNetResult() { Data = result };
        }


        public JsonNetResult Registro(int registro)
        {
            if (registro > 0)
            {
                var solicitud = _expedienteManager.GetExpedienteDerechoAutorPorRegistro(registro);

                var opciones = _opcionManager.GetOpcionesPorEstatus(solicitud.Expediente.EstatusId);

                result.Result = new
                {
                    documento = solicitud,
                    opciones = opciones
                };
            }

            return new JsonNetResult() { Data = result };
        }

        // GET: /Test/1 >userid=1
        public JsonNetResult ExpedienteId(int Id)
        {
            var solicitud = _expedienteManager.GetExpedienteDerechoAutorPorId(Id);
            var opciones = _opcionManager.GetOpcionesPorEstatus(solicitud.Expediente.EstatusId);

            result.Result = new
            {
                documento = solicitud,
                opciones = opciones
            };

            return new JsonNetResult() { Data = result };
        }


        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveSolicitud(GenericEntity<ExpedienteDAutor> model)
        {
            var doc = model.Generic as ExpedienteDAutor; //nombre, pais

            if (doc.Expediente.Id == -1)
            {
                var cronologia = new ModelCronologia(); // footprint separate table
                cronologia.Fecha = doc.Expediente.FechaDeSolicitud;
                doc.Expediente.EstatusId = cronologia.EstatusId = (int)DAExpedienteEstatus.Solicitud_Ingresada;
                doc.Expediente.FechaDeEstatus = DateTime.Now;
                cronologia.Referencia = "";
                cronologia.UsuarioId = sessionToken.UsuarioId;
                doc.Cronologia = new List<ModelCronologia>();
                doc.Cronologia.Add(cronologia);
            }

            doc.Expediente.ModuloId = (int)Modulos.DERECHO_DE_AUTOR;
            model.Extra = makeAuditoria(model.Auditoria, (doc.Expediente.Id == 0 ? "Agregó" : "Modificó") + "Expediente", doc.Expediente.Id);

            result = _derechoDeAutorManager.SaveSolicitud(model);
            var expedienteId = (int)result.Result;

            //var solicitud = _expedienteManager.GetExpedienteDerechoAutorPorId(expedienteId);

            //result.Result = new
            //{
            //    documento = solicitud
            //};
            return new JsonNetResult() { Data = result };
        }


        /// 
        /// AUTOR
        /// 
        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveAutor(GenericEntity<Autor> model)
        {
            var autor = model.Generic as Autor; //Autor data
            model.Extra = makeAuditoria(model.Auditoria, "Agregó Autor", autor.ExpedienteId);

            result = _derechoDeAutorManager.SaveAutor(model);
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult DeleteAutor(GenericEntity<Autor> model)
        {
            var autor = model.Generic as Autor; //Autor data
            model.Extra = makeAuditoria(model.Auditoria, "Eliminó Autor", autor.ExpedienteId);

            result = _derechoDeAutorManager.DeleteAutor(model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        /// 
        /// FonogramaTituloDeObra
        /// 
        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model)
        {
            var entry = model.Generic as FonogramaTituloDeObra; //FonogramaTituloDeObra data

            model.Extra = makeAuditoria(model.Auditoria, "Agregó FonogramaTituloDeObra", entry.ExpedienteId);

            result = _derechoDeAutorManager.SaveFonogramaTituloDeObra(model);
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult DeleteFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model)
        {
            var entry = model.Generic as FonogramaTituloDeObra; //FonogramaTituloDeObra data

            model.Extra = makeAuditoria(model.Auditoria, "Eliminó FonogramaTituloDeObra", entry.ExpedienteId);

            result = _derechoDeAutorManager.DeleteFonogramaTituloDeObra(model);

            return Json(result, JsonRequestBehavior.DenyGet);
        }


        /// 
        /// FonogramaArtista
        /// 
        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveFonogramaArtista(GenericEntity<FonogramaArtista> model)
        {
            var entry = model.Generic as FonogramaArtista; //FonogramaArtista data

            model.Extra = makeAuditoria(model.Auditoria, "Agregó FonogramaArtista", entry.ExpedienteId);

            result = _derechoDeAutorManager.SaveFonogramaArtista(model);
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult DeleteFonogramaArtista(GenericEntity<FonogramaArtista> model)
        {
            var entry = model.Generic as FonogramaArtista; //FonogramaArtista data
            model.Extra = makeAuditoria(model.Auditoria, "Eliminó FonogramaArtista", entry.ExpedienteId);

            result = _derechoDeAutorManager.DeleteFonogramaArtista(model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        /// 
        /// GuionAutor
        /// 
        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveGuionAutor(GenericEntity<GuionAutor> model)
        {
            var entry = model.Generic as GuionAutor; //GuionAutor data

            model.Extra = makeAuditoria(model.Auditoria, "Agregó GuionAutor", entry.ExpedienteId);

            result = _derechoDeAutorManager.SaveGuionAutor(model);
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult DeleteGuionAutor(GenericEntity<GuionAutor> model)
        {
            var entry = model.Generic as GuionAutor; //GuionAutor data
            model.Extra = makeAuditoria(model.Auditoria, "Eliminó GuionAutor", entry.ExpedienteId);

            result = _derechoDeAutorManager.DeleteGuionAutor(model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }


        /// 
        /// AudiovisualAutores
        /// 
        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveAudiovisualAutor(GenericEntity<AudiovisualAutor> model)
        {
            var entry = model.Generic as AudiovisualAutor; //AudiovisualAutor data

            model.Extra = makeAuditoria(model.Auditoria, "Agregó AudiovisualAutor", entry.ExpedienteId);

            result = _derechoDeAutorManager.SaveAudiovisualAutor(model);
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult DeleteAudiovisualAutor(GenericEntity<AudiovisualAutor> model)
        {
            var entry = model.Generic as AudiovisualAutor; //AudiovisualAutor data
            model.Extra = makeAuditoria(model.Auditoria, "Eliminó AudiovisualAutor", entry.ExpedienteId);

            result = _derechoDeAutorManager.DeleteAudiovisualAutor(model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// 
        /// ComposicionAutores
        /// 
        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult SaveComposicionAutor(GenericEntity<ComposicionAutor> model)
        {
            var entry = model.Generic as ComposicionAutor; //ComposicionAutor data

            model.Extra = makeAuditoria(model.Auditoria, "Agregó ComposicionAutor", entry.ExpedienteId);

            result = _derechoDeAutorManager.SaveComposicionAutor(model);
            return new JsonNetResult() { Data = result };
        }

        [HttpPost]
        [CheckSession(item = "DA995,DA994")]
        public JsonResult DeleteComposicionAutor(GenericEntity<ComposicionAutor> model)
        {
            var entry = model.Generic as ComposicionAutor; //ComposicionAutor data
            model.Extra = makeAuditoria(model.Auditoria, "Eliminó ComposicionAutor", entry.ExpedienteId);

            result = _derechoDeAutorManager.DeleteComposicionAutor(model);
            return Json(result, JsonRequestBehavior.DenyGet);
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

/*
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
            }
*/

            return tramite;
        }

        private ActionResult resolucion(string item, ResolucionCustomizada tramite)
        {

            var solicitud = _expedienteManager.GetExpedienteDerechoAutorPorId(tramite.ExpedienteId);
            tramite.DOCUMENT = JObject.Parse(tramite.JSONDOC);
            tramite = StampResolucion(tramite);

            switch (item)
            {
                case "DA001": //ConLugar
                    tramite.EstatusId = (int)DAExpedienteEstatus.Se_declara_con_Lugar;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Pendiente_de_Registrar;
                    break;
                case "DA002": //Rechazo
                    tramite.EstatusId = (int)DAExpedienteEstatus.Rechazo_de_plano;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Rechazo_de_plano;
                    break;
                case "DA003": //Admision_Tramite            
                    tramite.EstatusId = (int)DAExpedienteEstatus.Suspenso_x_Requerimiento;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Suspenso_x_Requerimiento;
                    break;
                case "DA004": //OperarMemorial            
                    tramite.EstatusId = (int)DAExpedienteEstatus.Memorial_Pendiente_de_Operar;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Memorial_Pendiente_de_Operar;
                    break;
                case "DA005": //LevantarSuspenso                       
                    tramite.EstatusId = (int)DAExpedienteEstatus.Levantar_Suspension_x_Sentencia;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Pendiente_de_Registrar;             
                    break;
                case "DA006": //RecursoRevocatoria                                   
                    tramite.EstatusId = (int)DAExpedienteEstatus.Recurso_de_Revocatoria;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Recurso_de_Revocatoria;            
                    break;
                case "DA007": //ElevandoRecurso            
                    tramite.EstatusId = (int)DAExpedienteEstatus.Elevando_Recurso_de_Revocatoria;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Elevando_Recurso_de_Revocatoria;            
                    break;
                case "DA008R": //PorRecibidoMINECOaRegistro                        
                    tramite.EstatusId = (int)DAExpedienteEstatus.Por_recibido_MINECO_a_Registro;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Por_recibido_MINECO_a_Registro;            
                    break;
                case "DA008A": //PorRecibidoMINECOArchivo                        
                    tramite.EstatusId = (int)DAExpedienteEstatus.Por_recibido_MINECO_a_Archivo;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Por_recibido_MINECO_a_Archivo;
                    break;
                case "DA009": //Notificacion                        
                    tramite.EstatusId = (int)DAExpedienteEstatus.Notificado;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Notificado;
                    break;
                case "DA010": //Titulo                        
                    tramite.EstatusId = (int)DAExpedienteEstatus.Registrada;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Registrada;
                    tramite.UpdatesEstatus = true;
                    tramite.Registrar = true;
                    break;
                case "DA011": //Cambio de Estatus
                    tramite.EstatusId = (int)tramite.EstatusId;
                    tramite.EstatusFinalId = (int)tramite.EstatusId;
                    tramite.UpdatesEstatus = true;
                    tramite.Observaciones = "Cambio de Estatus";
                    break;
                case "DA012": //Reposición Titulo                        
                    tramite.EstatusId = (int)DAExpedienteEstatus.Registrada;
                    tramite.EstatusFinalId = (int)DAExpedienteEstatus.Registrada;
                    tramite.UpdatesEstatus = false;
                    tramite.Observaciones = "Reposición del Título";
                    tramite.IsRepo = true;
                    tramite.Folio = solicitud.DerechoDeAutor.Folio;
                    tramite.Tomo = solicitud.DerechoDeAutor.Tomo;
                    tramite.Libro = solicitud.DerechoDeAutor.Libro;
                    tramite.Registro = solicitud.DerechoDeAutor.Registro;
                    break;            
            }
            tramite.UpdatesEstatus = true;

            ViewBag.Solicitud = solicitud;
            ViewBag.Tramite = tramite;

            var htmlString = String.Empty;
            switch (item)
            {
                case "DA001": //ConLugar
                    htmlString = RenderRazorViewToString("ConLugar", null);
                    break;
                case "DA002": //Rechazo
                    htmlString = RenderRazorViewToString("Rechazo", null);
                    break;
                case "DA003": //Admision_Tramite
                    htmlString = RenderRazorViewToString("Suspenso", null);
                    break;
                case "DA004": //OperarMemorial
                    htmlString = RenderRazorViewToString("OperarMemorial", null);
                    break;
                case "DA005": //LevantarSuspenso
                    htmlString = RenderRazorViewToString("LevantarSuspenso", null);
                    break;
                case "DA006": //RecursoRevocatoria
                    htmlString = RenderRazorViewToString("RecursoRevocatoria", null);
                    break;
                case "DA007": //ElevandoRecurso
                    htmlString = RenderRazorViewToString("ElevandoRecurso", null);
                    break;
                case "DA008R": //PorRecibidoMINECOaRegistro
                    htmlString = RenderRazorViewToString("PorRecibidoMINECOaRegistro", null);
                    break;
                case "DA008A": //PorRecibidoMINECOArchivo
                    htmlString = RenderRazorViewToString("PorRecibidoMINECOArchivo", null);
                    break;
                case "DA009": //Notificacion                                    
                    htmlString = RenderRazorViewToString("Notificacion", null);
                    break;
                case "DA010": //Titulo                        
                    htmlString = RenderRazorViewToString("Titulo", null);
                    break;
                case "DA011": //Cambio de Estatus
                    htmlString = "Cambio de Estatus";
                    break;
                case "DA012": //Reposición Titulo                        
                    htmlString = RenderRazorViewToString("Titulo", null);
                    break;
            }

            tramite.HTMLDOC = htmlString;
            var cronologiaResult = _expedienteManager.SaveEventoCronologicoDeDA(tramite);

            // save again and update res...
            return Content(htmlString, "text/html");

        }





        /// <summary>
        /// CON LUGAR
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA001")]
        //public ContentResult ConLugar(ResolucionCustomizada tramite)
        public ActionResult ConLugar(ResolucionCustomizada tramite)            
        {
            return resolucion("DA001", tramite);
        }

        /// <summary>
        /// RECHAZO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA002")]
        public ActionResult Rechazo(ResolucionCustomizada tramite)
        {
            return resolucion("DA002", tramite);
        }

        /// <summary>
        /// SUSPENSO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA003")]
        public ActionResult Suspenso(ResolucionCustomizada tramite)
        {
            return resolucion("DA003", tramite);
        }

        /// <summary>
        /// OPERAR MEMORIAL
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA004")]
        public ActionResult OperarMemorial(ResolucionCustomizada tramite)
        {
            return resolucion("DA004", tramite);
        }


        /// <summary>
        /// LEVANTAR SUSPENSO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA005")]
        public ActionResult LevantarSuspenso(ResolucionCustomizada tramite)
        {
            return resolucion("DA005", tramite);
        }


        /// <summary>
        /// RECURSO REVOCATORIA
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA006")]
        public ActionResult RecursoRevocatoria(ResolucionCustomizada tramite)
        {
            return resolucion("DA006", tramite);
        }


        /// <summary>
        /// ELEVANDO RECURSO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA007")]
        public ActionResult ElevandoRecurso(ResolucionCustomizada tramite)
        {
            return resolucion("DA007", tramite);
        }


        /// <summary>
        /// POR RECIBIDO MINECO - Registro
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA008R")]
        public ActionResult PorRecibidoMINECOaRegistro(ResolucionCustomizada tramite)
        {
            return resolucion("DA008R", tramite);
        }

        /// <summary>
        /// POR RECIBIDO MINECO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA008A")]
        public ActionResult PorRecibidoMINECOArchivo(ResolucionCustomizada tramite)
        {
            return resolucion("DA008A", tramite);
        }

        /// <summary>
        /// NOTIFICAR
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA009")]
        public ActionResult Notificar(ResolucionCustomizada tramite)
        {
            return resolucion("DA009", tramite);
        }


        /// <summary>
        /// EMITIR TITULO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA010")]
        public ActionResult EmitirTitulo(ResolucionCustomizada tramite)
        {

            return resolucion("DA010", tramite);
        }

        /// <summary>
        /// CAMBIAR ESTATUS
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA011")]
        public ActionResult CambiarEstatus(ResolucionCustomizada tramite)
        {
            return resolucion("DA011", tramite);
        }

        /// <summary>
        /// REPOSICION DE TITULO
        /// </summary>
        [HttpPost]
        [CheckSession(item = "DA012")]
        public ActionResult ReposiciondeTitulo(ResolucionCustomizada tramite)
        {
            return resolucion("DA012", tramite);
        }
    }
}
