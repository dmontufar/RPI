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
    /// <summary>
    /// PUBLICO
    /// </summary>
    public class AuthPublicController : CoreController
    {
        private IUsuarioPublicoManager _userManager;
        private int footMark = 2;

        public AuthPublicController(IUsuarioPublicoManager usuarioPublicoManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _userManager = usuarioPublicoManager;
        }

        public JsonResult Login(string userName, string password)
        {

            ResultInfo authResult = _userManager.Find(userName, password);
            var login = (UsuarioPublico)authResult.Result;

            result.Succeeded = authResult.Succeeded;
            if (result.Succeeded)
            {
                var sessionResult = sessionManager.SetTokenForUserId(login.Id, footMark); // Usuario Publico
                result.Result = new
                {
                    token = ((Session)sessionResult.Result).Token,
                    Nombre = login.Nombre,
                    Email = login.Cuenta,
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Logout()
        {
            string token = System.Web.HttpContext.Current.Request.Headers["access_token"];
            string result = "logged out";

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}