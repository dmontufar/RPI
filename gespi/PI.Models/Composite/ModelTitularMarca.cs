using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class ModelTitularMarca
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int TitularId { get; set; }
        public string DireccionParaNotificacion { get; set; }
        public string DireccionParaUbicacion { get; set; }
        public string EnCalidadDe { get; set; }
        public int PaisId { get; set; }
    }
}
