using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class ExpedienteDeAnotaciones
    {
        public Expediente Expediente { get; set; }
        public Anotacion Anotacion { get; set; }
        public Estatus Estatus { get; set; }
        public List<ModelCronologia> Cronologia { get; set; }
        public List<ModelAnotacionEnExpedientes> ExpedientesConAnotacion { get; set; } //expedientes de marca con anotacion 
        public string SituacionAdmin { get; set; }


        public string getSello(string llave)
        {
            return "#Id:" + llave + "#Exp:" + this.Expediente.Numero + "#Dsc:" + this.Anotacion.NuevoTitular + "#Fecha:" + this.Expediente.FechaDeEstatus.ToShortDateString() + "#Emisor:YOLODEMO";
        }

        public string getFirma()
        {
            return "#Exp:" + this.Expediente.Numero + "#Nombre:NOMBRE FIRMANTE" + "#Emisor:CODIGO";
        }


        public static ExpedienteDeAnotaciones CreateExpediente() 
        {
            var solicitud = new ExpedienteDeAnotaciones()
            {
                Expediente = new Expediente(),
                Anotacion = new Anotacion(),
                Cronologia = new List<ModelCronologia>(),
                ExpedientesConAnotacion = new List<ModelAnotacionEnExpedientes>(),
            };
            return solicitud;
        }
    }

}
