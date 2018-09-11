using PI.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class CryptoManager: ICryptoManager
    {
        private byte[] _salt = System.Text.Encoding.Default.GetBytes("Gestion.p.i.");

        public string Encrypt(string text)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(text + _salt);

            SHA1 sha = new SHA1CryptoServiceProvider(); 
            var result = sha.ComputeHash(data);
            return System.Text.Encoding.Default.GetString(result);
        }
    }
}
