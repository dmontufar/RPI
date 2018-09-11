using Common.Web;
using PI.Baktun.Controllers;
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

namespace PI.Baktun.Areas.Admin.Controllers
{
    public class UsuariosPublicosController : CoreController
    {
        private IUsuarioPublicoManager _usuarioPublicoManager;

        public UsuariosPublicosController(IUsuarioPublicoManager usuarioPublicoManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _usuarioPublicoManager = usuarioPublicoManager;
            result.Succeeded = false;
        }

        // GET: /Test/1 >userid=1
        public JsonResult Index(int id = 0)
        {
            if (IsAuth)
            {
                if (id == 0)
                {
                    result.Succeeded = true;
                    result.Result = new UsuarioPublico() { Id = -1 };
                }
                else
                {
                    result = _usuarioPublicoManager.Get(id);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadMyPerfil()
        {
            if (IsAuth)
            {
                result = _usuarioPublicoManager.Get(sessionToken.UsuarioId);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: /Test/1 >userid=1
        public JsonResult GetWithSpk(string spk, int id = 0)
        {
            if (id > 0)
            {
                result = _usuarioPublicoManager.Get(id);
                var usr = (UsuarioPublico)result.Result;
                if (usr.Spk != spk || usr.SpkExpiration < DateTime.Now)
                {
                    result.Result = null;
                    result.Succeeded = false;
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult ResetPW(UsuarioPublico model)
        {
            if (model.Id > 0)
            {
                result = _usuarioPublicoManager.SetPassword(model);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Save(UsuarioPublico model)
        {
            if (IsAuth)
            {
                string key1 = Util.RandomString(4);
                string key2 = Util.RandomString(4);
                string fullkey = key1 + "-" + key2;
                model.Spk = fullkey;
                model.SpkExpiration = DateTime.Now.AddHours(24);
                if (model.Id == -1)
                {
                    model.Pwd = fullkey; //invalid pw when creating a user
                    result = _usuarioPublicoManager.Create(model);
                }
                else
                {
                    var dbUser = _usuarioPublicoManager.Get(model.Id);
                    var usr = (UsuarioPublico)dbUser.Result;
                    model.Pwd = usr.Pwd;

                    result = _usuarioPublicoManager.Update(model);
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetPage(int page = 1, int pageSize = 10)
        {
            if (IsAuth)
                result = _usuarioPublicoManager.GetPage(page, pageSize, null, order => order.Nombre);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPageFilter(string filter, int page = 1, int pageSize = 0)
        {
            if (IsAuth)
                result = _usuarioPublicoManager.GetPage(page, pageSize, f => f.Nombre.Contains(filter), order => order.Nombre);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Login(string userName, string password)
        {
            ResultInfo result = _usuarioPublicoManager.Find(userName, password);
            var login = result.Result;
            var sessionResult = sessionManager.SetTokenForUserId(((UsuarioPublico)result.Result).Id, 2); // Usuario Publico

            result.Result = new { Usuario = login, token = ((Session)sessionResult.Result).Token };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateMasterUser()
        {
            var user = new UsuarioPublico()
            {
                Nombre = "MasterUser",
                Cuenta = "email@gmail.com",
                Pwd = "123"
            };

            var resultX = _usuarioPublicoManager.SignIn(user);
            return Json(resultX, JsonRequestBehavior.AllowGet);
        }

    }
}
