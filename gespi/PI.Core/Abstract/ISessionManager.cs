using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface ISessionStateManager
    {
        void Set(string sessionKey, object sessionValue);

        object Get(string sessionKey);
    }
}
