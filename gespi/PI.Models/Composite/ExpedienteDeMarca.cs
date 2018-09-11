using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class ExpedienteDeMarca
    {
        public Expediente Expediente { get; set; }
        public Marca Marca { get; set; }
        public List<PrioridadMarca> Prioridades { get; set; }
        public ProductosMarca Productos { get; set; }
        public List<ModelTitularMarca> Titulares { get; set; }
        public List<Renovacion> Renovaciones { get; set; }
        public List<Anotacion> Anotaciones { get; set; }
        public List<ModelCronologia> Cronologia { get; set; }
        public ClassificacionDeNiza ClassificacionDeNiza { get; set; }
        public List<ModelViena> ClasificacionDeViena { get; set; }
        public string SituacionAdmin { get; set; }
        public string TipoDeRegistro { get; set; }
        public string TipoDeMarca { get; set; }
        //public List<Digital> Digital { get; set; }

        public string getSello(string llave)
        {
            return "#Id:" + llave + "#Exp:" + this.Expediente.Numero + "#Dsc:" + this.Marca.Denominacion + "#Fecha:" + this.Expediente.FechaDeEstatus.ToShortDateString() + "#Emisor:YOLODEMO";
        }

        public string getFirma()
        {
            return "#Exp:" + this.Expediente.Numero + "#Nombre:NOMBRE FIRMANTE" + "#Emisor:CODIGO";
        }


        public static ExpedienteDeMarca CreateExpediente() 
        {
            var solicitud = new ExpedienteDeMarca()
            {
                Expediente = new Expediente(),
                Marca = new Marca(),
                ClassificacionDeNiza = new ClassificacionDeNiza(),
                Cronologia = new List<ModelCronologia>(),
                Titulares = new List<ModelTitularMarca>()
            };
            return solicitud;
            
        }
    }
}
