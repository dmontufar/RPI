using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class BusquedaExpedienteMarca
    {
        public int ExpedienteId { get; set; }
        public string Numero { get; set; }
        public string SignoDistintivo { get; set; }
        public string TipoDeRegistro { get; set; }
        public string ClasificacionNiza { get; set; }
        public string EstatusDsc { get; set; }
        public int Punteo { get; set; }
        public int Registro { get; set; }
    }
}
