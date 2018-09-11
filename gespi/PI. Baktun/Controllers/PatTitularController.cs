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
    public class PatTitularController : CoreController
    {
        private IPatTitularManager _patTitularManager;

        public PatTitularController(IPatTitularManager patTitularManager, ISessionManager sessionManager)
            : base(sessionManager)
        {
            _patTitularManager = patTitularManager;
        }

        public JsonNetResult Search(string textToSearch)
        {
            if (IsAuth)
            {
                result = _patTitularManager.searchTitular(textToSearch);
            }

            return new JsonNetResult() { Data = result };
        }

    }
}
