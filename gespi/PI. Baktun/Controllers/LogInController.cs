using PI.Core;
using PI.Core.Abstract;
using PI.Internal.Model;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI.Baktun.Controllers
{
    public class LoginController : Controller
    {
        private IUserManager _userManager;

        public LoginController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        //
        // GET: /Home/

        [AllowAnonymous]
        public ActionResult Index(string redirectToUrl)
        {
            var model = new LoginModel();
            ViewBag.redirectToUrl = redirectToUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            var user = new Usuario()
            {
                Email = "email@gmail.com",
                Nombre = "email",
                Password = "3m@1l",
                RolId = 1,
                Salt = "x"
            };

            //var resultX = _userManager.Create(user);
            
            var result = _userManager.Find(model.Account, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.RedirectToUrl))
                    return Redirect(model.RedirectToUrl);

                return RedirectToAction("Menu", "Base"); 
            }

            // TODO: include erros in modelstate
            ModelState.AddModelError("", "Cuenta de usuario o clave invalida...");

            return View(model);
        }

    }
}
