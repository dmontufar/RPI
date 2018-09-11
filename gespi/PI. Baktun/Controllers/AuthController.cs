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

namespace PI.Baktun.Controllers
{
    public class AuthController : CoreController
    {
        private IUserManager _userManager;
        private int footMark = 1;

        public AuthController(IUserManager userManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _userManager = userManager;
        }

        public JsonResult Login(string userName, string password)
        {
            ResultInfo authResult = _userManager.Find(userName, password);
            var userSettings = (UserSettings)authResult.Result;

            result.Succeeded = authResult.Succeeded;
            if (result.Succeeded)
            {
                var sessionResult = sessionManager.SetTokenForUserId(userSettings.Usuario.Id, footMark); // Usuario Publico
                result.Result = new
                {
                    token = ((Session)sessionResult.Result).Token + "-I",
                    Nombre = userSettings.Usuario.Nombre,
                    Email = userSettings.Usuario.Email,
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