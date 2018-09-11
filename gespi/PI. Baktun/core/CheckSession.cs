using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI.Core;
using PI.Models;
using PI.Common;
using Autofac;


namespace PI.Baktun.core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckSessionAttribute : AuthorizeAttribute
    {

        public ISessionManager _sessionManager { get; set; }
        private static IContainer Container { get; set; }
        public String item { get; set; }

        public CheckSessionAttribute() {
            _sessionManager = DependencyResolver.Current.GetService<ISessionManager>();            
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {            
            string token = System.Web.HttpContext.Current.Request.Headers["access_token"];
            System.Diagnostics.Debug.WriteLine("token:" + token);
            var signature = !string.IsNullOrEmpty(token) && token.Length > 9 ? 1 : 2;
            if (string.IsNullOrEmpty(token) || token == "null")
                return false;

            return isAuthorized(string.IsNullOrEmpty(token) ? "" : token.Substring(0, 9), signature);
        }

        private bool isAuthorized(string token, int signature)
        {
            ResultInfo result = (item=="n/a") ?
                result = _sessionManager.GetByToken(token, signature)
                :
                result = _sessionManager.isAuthorized(token, signature, item);

            System.Diagnostics.Debug.WriteLine("item:" + item);

            return result.Succeeded;
        }
    }
}