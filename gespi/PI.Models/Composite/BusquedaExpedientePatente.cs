using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class BusquedaExpedientePatente
    {
        public int ExpedienteId { get; set; }
        public string Numero { get; set; }
        public string Descripcion { get; set; }
        public string TipoDeRegistro { get; set; }
        public int Registro { get; set; }
        public int Punteo { get; set; }
    }
}
