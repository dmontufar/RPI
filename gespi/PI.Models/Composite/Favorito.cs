using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class MiFavorito
    {
        public int ExpedienteId { get; set; }
        public string Numero { get; set; }
        public string Notas { get; set; }
        public string TipoDeRegistro { get; set; }
        public DateTime FechaDeSolicitud { get; set; }
        public string EstatusDsc { get; set; }
        public DateTime FechaDeEstatus { get; set; }
        public string Denominacion { get; set; }
        public string Descripcion { get; set; }
        public string Titulo { get; set; }
        public int ModuloId { get; set; }
    }
}
