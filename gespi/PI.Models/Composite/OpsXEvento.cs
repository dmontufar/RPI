using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public partial class OpsXEvento
    {
        public int Id { get; set; }
        public Nullable<int> EstatusActualId { get; set; }
        public string Estatus { get; set; }
        public Nullable<int> OpcionId { get; set; }
        public string Opcion { get; set; }
    }
}
