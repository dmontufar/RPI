using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class GenericEntity<T> where T : new()
    {
        public T Generic { get; set; }
        public dynamic Extra { get; set; }
        public string jsExtra { get; set; }
        public string Auditoria { get; set; }
    }
}
