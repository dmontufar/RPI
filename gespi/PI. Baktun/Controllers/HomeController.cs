using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PI.Common;
using PI.Core;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }

    public class CoreController : Controller
    {
        public ISessionManager sessionManager;

        public bool IsAuth { get; set; }
        public Session sessionToken { get; set; }
        public string usrName { get; set; }
        public ResultInfo result = new ResultInfo();

        public CoreController(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
            string token = System.Web.HttpContext.Current.Request.Headers["access_token"];
             System.Diagnostics.Debug.WriteLine("token:" + token);
            var signature = !string.IsNullOrEmpty(token) && token.Length > 9 ? 1 : 2;
            sessionToken = GetSession(string.IsNullOrEmpty(token) ? "" : token.Substring(0,9), signature);
            IsAuth = (!string.IsNullOrEmpty(token) && token != "null"); //&& sessionToken != null
            result.Succeeded = IsAuth;
            result.Errors = (!IsAuth ? "Invalid Token" : "");
        }

        private Session GetSession(string token, int signature)
        {
            ResultInfo result = sessionManager.GetByToken(token, signature);
            if (result.Succeeded)
            {
                return (Session)result.Result;
            }
            return null;
        }

        public ISessionManager getSessionManager() {
            return sessionManager;
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ResolucionCustomizada StampResolucion(ResolucionCustomizada tramite)
        {
            tramite.Llave = Guid.NewGuid();
            tramite.UsuarioId = sessionToken.UsuarioId;
            return tramite;
        }

        /// <summary>
        /// Auditoria
        /// </summary>
        public Auditoria makeAuditoria(string historial, string evento, int expedienteId)
        {
            return new Auditoria()
            {
                UsuarioId = sessionToken.UsuarioId,
                Evento = evento,
                ExpedienteId = expedienteId,
                Fecha = DateTime.Now,
                Historial = historial
            };
        }

    }

    //http://www.developer.com/net/dealing-with-json-dates-in-asp.net-mvc.html
    public class JsonNetResult : JsonResult
    {
        public new object Data { get; set; }

        public JsonNetResult()
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
            {
                //IsoDate
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
                using (JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting.Indented, DateFormatHandling = DateFormatHandling.IsoDateFormat })
                {
                    serializer.Serialize(writer, Data);
                }

                //JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
                //serializer.Converters.Add(new epochDateTimeConverter());
                //serializer.NullValueHandling = NullValueHandling.Ignore;

                //using (JsonTextWriter writer = new JsonTextWriter(response.Output) { })
                //{
                //    serializer.Serialize(writer, Data);
                //}
            }
        }
    }


    public class epochDateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer)
            {
                throw new Exception(
                    String.Format("Unexpected token parsing date. Expected Integer, got {0}.",
                    reader.TokenType));
            }

            var ticks = (long)reader.Value;

            var date = new DateTime(1970, 1, 1);
            date = date.AddMilliseconds(ticks);

            return date;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            long ticks;
            if (value is DateTime)
            {
                var epoch = new DateTime(1970, 1, 1);
                var delta = ((DateTime)value) - epoch;
                ticks = (long)delta.TotalMilliseconds;
            }
            else
            {
                throw new Exception("Expected date object value.");
            }
            writer.WriteValue(ticks);
        }
    }



}