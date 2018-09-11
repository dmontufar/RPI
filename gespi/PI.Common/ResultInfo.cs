using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Common
{
    public class ResultInfo
    {
        public string Errors { get; set; }
        public bool Succeeded { get; set; }
        public object Result { get; set; }
        public Type type { get; set; }
    }
}
