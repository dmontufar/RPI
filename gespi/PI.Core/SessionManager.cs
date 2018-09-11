using PI.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PI.Core
{
   public class SessionSateManager : ISessionStateManager
    {
        private readonly string _errhttpContextNotAvailable = "HttpContext Not Available";
        private readonly HttpContextBase _httpContext;

        public SessionSateManager()
        {
            // could be better :P
            _httpContext = (new HttpContextWrapper(HttpContext.Current) as HttpContextBase);      
        }

        public void Set(string sessionKey, object sessionValue)
        {
            if (_httpContext == null)
            {
                throw new Exception(_errhttpContextNotAvailable);
            }

            var currentSessionValue = _httpContext.Session[sessionKey];

            if (currentSessionValue == null)
            {
                _httpContext.Session[sessionKey] = sessionValue;
            }
            else
            {
                //log overriding session value
            }
        }


        public object Get(string sessionKey)
        {
            if (_httpContext == null)
            {
                throw new Exception(_errhttpContextNotAvailable);
            }

            var currentSessionValue = _httpContext.Session[sessionKey];

            return currentSessionValue;
        }

    }
}

