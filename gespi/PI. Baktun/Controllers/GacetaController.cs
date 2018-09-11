using PI.Common;
using PI.Core;
using PI.Core.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class GacetaController : CoreController
    {

        private IPublicacionManager _publicacionManager;

        public GacetaController(IPublicacionManager publicacionManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _publicacionManager = publicacionManager;
        }

        public JsonNetResult GetPage(int page = 1, int pageSize = 100)
        {
            if (IsAuth)
                result = _publicacionManager.GetPage(page, pageSize, null, order => order.fecha_publicacion);

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult GetPageFilter(string filter, int page = 1, int pageSize = 100)
        {
            result = _publicacionManager.GetPage(page, pageSize, f => f.distintivo.ToLower().Contains(filter.ToLower()), order => order.fecha_publicacion);

            return new JsonNetResult() { Data = result };
        }

        public JsonNetResult GetPageFilterByDate(string fechaPublicacion, int page = 1, int pageSize = 100)
        {
            DateTime? date = null;

            IFormatProvider culture = new System.Globalization.CultureInfo("es-GT", true);
            if (!string.IsNullOrEmpty(fechaPublicacion))
                date = DateTime.Parse(fechaPublicacion, culture, System.Globalization.DateTimeStyles.AssumeLocal);

            result = _publicacionManager.GetPage(page, pageSize, f => f.fecha_publicacion == date, order => order.fecha_publicacion);

            return new JsonNetResult() { Data = result };
        }

        public ActionResult Edicto(int Id)
        {
            result = _publicacionManager.Get(Id);            
            ViewBag.Many = false;
            return View(result.Result);
        }


        public ActionResult EdictoAsPdf(int Id)
        {
            ViewBag.Many = false;
            result = _publicacionManager.Get(Id);     
            var rotativaOptions = new Rotativa.Core.DriverOptions()
            {
                CustomSwitches = "--print-media-type",
                PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Core.Options.Margins(1,0,1,0),
                PageWidth = 215.9,
                PageHeight = 279.4
            };

            return new Rotativa.MVC.ViewAsPdf("Edicto", result.Result)
            {
                FileName = "Edicto"+Id.ToString()+".Pdf",
                RotativaOptions = rotativaOptions
            };
        }

        public ActionResult Semanal(string fecha, string format)
        {
            DateTime? date = null;

            IFormatProvider culture = new System.Globalization.CultureInfo("es-GT", true);
            if (!string.IsNullOrEmpty(fecha))
                date = DateTime.Parse(fecha, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            else
                date = DateTime.Today;

            var startDate = date.Value.AddDays(1 - (int)(date.Value.DayOfWeek));
            var endDate = startDate.AddDays(4);
            result.Result = new { startDate = startDate, endDate = endDate };

            result = _publicacionManager.GetPage(0, 0, e => e.fecha_publicacion >= startDate && e.fecha_publicacion <= endDate && e.fecha_publicacion <= DateTime.Today, order => order.fecha_publicacion);

            ViewBag.Many = true;
            var week = startDate.ToLongDateString() + " - " + endDate.ToLongDateString();
            ViewBag.Week = week;

            if (!string.IsNullOrEmpty(format) && format=="pdf")
            {
                var rotativaOptions = new Rotativa.Core.DriverOptions()
                {
                    CustomSwitches = "--print-media-type",
                    PageOrientation = Rotativa.Core.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Core.Options.Margins(1, 0, 1, 0),
                    PageWidth = 215.9,
                    PageHeight = 279.4
                };

                return new Rotativa.MVC.ViewAsPdf("Semanal", ((PagedList)result.Result).DataSet)
                {
                    FileName = "Gaceta_" + week + ".pdf",
                    RotativaOptions = rotativaOptions
                };
            }
            return View(((PagedList)result.Result).DataSet);
        }



        //public ActionResult Gaceta(string weekId)
        //{
        //    var week = WeekRepository.ListOfWeeks.Where(w => w.Id == weekId).First();

        //    var edictos = PublicacionRepository.GetEdictosPorSemana(week.StartDate, week.EndDate);
        //    ViewBag.Many = true;
        //    ViewBag.Week = week;
        //    return View(edictos);
        //}

        ////[CheckSession] Only if you pay
        //public ActionResult GacetaAsPdf(string weekId)
        //{
        //    var week = WeekRepository.ListOfWeeks.Where(w => w.Id == weekId).First();

        //    var edictos = PublicacionRepository.GetEdictosPorSemana(week.StartDate, week.EndDate);

        //    return new Rotativa.ActionAsPdf("Gaceta", new { weekId = weekId })
        //    {
        //        FileName = "Gaceta_" + week.Description + ".pdf",
        //        CustomSwitches = "--print-media-type",
        //        PageOrientation = Rotativa.Options.Orientation.Portrait,
        //        PageMargins = { Left = 0, Right = 0 },
        //        PageWidth = 215.9,
        //        PageHeight = 279.4
        //    };
        //}        


        ////
        //// GET: /Gaceta/
        //public ActionResult Index(DateTime? fechaPublicacion)
        //{
        //    //var model = Publicacion.DummyData.Where(p=>p.FechaPublicacion == DateTime.Today).ToList();
        //    var model = PublicacionRepository.GetEdictos(0, fechaPublicacion);
        //    return View(model);
        //}


        //public JsonResult Calculo(string expediente)
        //{
        //    var edicto = PublicacionRepository.GetEdictoByExpedienteId(expediente);
        //    View(edicto).ExecuteResult(base.ControllerContext);
        //    return Json(TempData["Edicto"].ToString().Length, JsonRequestBehavior.AllowGet);
        //    //return View(edicto)
        //}

        //public ActionResult CalculoAsPdf(string expediente)
        //{
        //    return new Rotativa.ActionAsPdf("Calculo", new { expediente = expediente })
        //    {
        //        FileName = "Calculo.pdf",
        //        CustomSwitches = "--print-media-type",
        //        PageOrientation = Rotativa.Options.Orientation.Portrait,
        //        PageMargins = { Left = 0, Right = 0 },
        //        PageWidth = 215.9,
        //        PageHeight = 279.4
        //    };
        //}

        ////[CheckSession] Only if you pay
        //public ActionResult Semanal()
        //{
        //    var model = WeekRepository.ListOfWeeks;
        //    return View(model);
        //}




        //public ActionResult Edicto(int Id)
        //{
        //    var edicto = PublicacionRepository.GetEdicto(Id);
        //    ViewBag.Many = false;
        //    return View(edicto);
        //}

        //public ActionResult EdictoAsPdf(int Id)
        //{

        //    var edicto = PublicacionRepository.GetEdicto(Id);

        //    return new Rotativa.ActionAsPdf("Edicto", new { Id = Id })
        //    {
        //        FileName = "Edicto.pdf",
        //        CustomSwitches = "--print-media-type",
        //        PageOrientation = Rotativa.Options.Orientation.Portrait,
        //        PageMargins = { Left = 0, Right = 0 },
        //        PageWidth = 215.9,
        //        PageHeight = 279.4
        //    }; ;
        //}

        //public ActionResult GetMore(int flag, string fechaPublicacion)
        //{
        //    DateTime? date = null;

        //    IFormatProvider culture = new System.Globalization.CultureInfo("es-GT", true);
        //    if (!string.IsNullOrEmpty(fechaPublicacion))
        //        date = DateTime.Parse(fechaPublicacion, culture, System.Globalization.DateTimeStyles.AssumeLocal);

        //    //var model = Publicacion.DummyData.Skip(101 + flag).Take(100).ToList();
        //    var model = PublicacionRepository.GetEdictos(flag, date);
        //    return PartialView(model);
        //}

    }
}
