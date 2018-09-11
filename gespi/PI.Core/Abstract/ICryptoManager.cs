using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface ICryptoManager
    {
        string Encrypt(string text);
    }
}
