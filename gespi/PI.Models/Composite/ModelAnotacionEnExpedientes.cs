using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class ModelAnotacionEnExpedientes : AnotacionEnExpedientes
    {
        //public string Tipo_Anotacion { get; set; }
        public string NumeroDeExpedienteMarca { get; set; }
        public string TipoDeRegistroDeLaMarca { get; set; }
        public string Denominacion { get; set; }
        public string NombreTitular { get; set; }
        public string FechaDeRegistro { get; set; }
        public string FechaDeRegistroEnMarcas { get; set; }
        public String ClasificacionNiza { get; set; }
    }
}
