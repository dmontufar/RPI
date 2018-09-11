using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class ModelCronologia
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int EstatusId { get; set; }
        public string EstatusDescripcion { get; set; }
        public string Referencia { get; set; }
        public string UsuarioIniciales { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioId { get; set; }
    }
}
