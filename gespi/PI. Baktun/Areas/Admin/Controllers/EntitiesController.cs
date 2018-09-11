using PI.Baktun.Controllers;
using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Internal.Models;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Areas.Admin.Controllers
{
    public class EntitiesController : CoreController
    {
        private IEstadisticaManager _estadisticasManager;
        public EntitiesController(ISessionManager sessionManager, IEstadisticaManager estadisticasManager)
            : base(sessionManager)
        {
            _estadisticasManager = estadisticasManager;
        }

        public JsonResult GetEstadisticasByArea(int year = 0)
        {
            var result = _estadisticasManager.GetEstadisticasByArea(year);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIngresoExpedientesPorMes(int year = 0)
        {
            var result = _estadisticasManager.GetIngresoExpedientesPorMes(year);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassificacionPat()
        {
            result.Result = EnumModels.GetClasificaciones();
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult Get(string entity)
        {
            //if (IsAuth)
            if (true)
            {
                switch (entity)
                {
                    case "paises":
                        result.Result = EnumModels.GetPaises();
                        break;
                    case "leyes":
                        result.Result = EnumModels.GetLeyes();
                        break;
                    case "tipos-de-patentes":
                        result.Result = EnumModels.GetTiposDePatente();
                        break;
                    case "tipos-de-obras":
                        result.Result = EnumModels.GetTiposDeObra();
                        break;
                    case "tipos-de-marcas":
                        result.Result = EnumModels.GetTiposDeRegistroDeMarca();
                        break;
                    case "tipos-de-anotaciones":
                        result.Result = EnumModels.GetTiposDeAnotaciones();
                        break;
                    case "all":
                        result = GetAll();
                        break;
                    default:
                        result.Result = new object();
                        break;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ResultInfo GetAll()
        {
            var result = new ResultInfo();

            result.Result = new
            {
                paises = EnumModels.GetPaises(),
                leyes = EnumModels.GetLeyes(),
                tipoDePatentes = EnumModels.GetTiposDePatente(),
                tipoDeObras =EnumModels.GetTiposDeObra(),
                tipoDeRegistroDeMarcas = EnumModels.GetTiposDeRegistroDeMarca(),
                tipoDeAnotaciones = EnumModels.GetTiposDeAnotaciones()
            };

            return result;
        }
    }
}
