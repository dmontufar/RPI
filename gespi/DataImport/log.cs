using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImport
{
    public class log
    {
        public static void Error(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Trace.TraceError(message);
        }
    }
}
