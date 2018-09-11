using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class ExpedienteDePatente
    {
        public Expediente Expediente { get; set; }
        public Patente Patente { get; set; }
        public Estatus Estatus { get; set; }
        public List<ModelCronologia> Cronologia { get; set; }
        public string Ipc1 { get; set; }
        public string Ipc2 { get; set; }
        public string Ipc3 { get; set; }
        public string Ipc4 { get; set; }
        public List<ModelTitular> Titulares { get; set; }
        public Clasificacion Clasificacion { get; set; }
        public Agente Agente { get; set; }
        public List<Inventor> Inventores { get; set; }
        public List<Anualidad> Anualidades { get; set; }
        public List<ModelPrioridad> Prioridades { get; set; }
        public string SituacionAdmin { get; set; }

        public class _patente //data formated for printing purposes 
        {
            public string tituloP  { get; set; }
            public string kind { get; set; }
            public string solicitante { get; set; }
            public bool esEnNombrePropio { get; set; }
        }

        public _patente getFORMATEDDATA()
        {
            _patente res = new _patente();

            switch (this.Expediente.TipoDeRegistroId)
            {
                case 10:
                    res.tituloP = "Nombre de la Invención";
                    if (int.Parse(this.Expediente.Numero)>=200600444){
                        res.kind = "A";
                    }
                    else{
                        res.kind = "PI";           
                    }
                    break;
                case 9:
                    res.tituloP = "Nombre del Modelo de Utilidad";            
                    if (int.Parse(this.Expediente.Numero)>=200600010){
                        res.kind = "U";
                    }
                    else{
                        res.kind = "MU";           
                    }            
                    break;                    
                case 7:
                    res.tituloP = "Nombre del Diseño Industrial";            
                    if (int.Parse(this.Expediente.Numero)>=200600063){
                        res.kind = "S";
                    }
                    else{
                        res.kind = "DI";           
                    }                        
                    break;                    
                case 6:
                    res.tituloP = "Nombre del Diseño Industrial";
                    res.kind = "F";
                    break;     
            }               
    
            res.solicitante = String.Empty;
            this.Titulares.ForEach(t => res.solicitante += t.Nombre + ", ");
            if (res.solicitante.Length > 0)
            {
                res.solicitante = res.solicitante.Substring(0, res.solicitante.Length - 2);
            }

            if (this.Agente == null || ( this.Agente != null && String.IsNullOrEmpty(this.Agente.Nombre))){
                res.solicitante = res.solicitante + ", quien actúa en nombre propio. ";
                res.esEnNombrePropio = true;
            }
            return res;
        }

        public string getSello(string llave)
        {
            return "#Id:" + llave + "#Exp:" + this.Expediente.Numero + "#Dsc:" + this.Patente.Descripcion + "#Fecha:" + this.Expediente.FechaDeEstatus.ToShortDateString() + "#Emisor:YOLODEMO";
        }

        public string getFirma()
        {
            return "#Exp:" + this.Expediente.Numero + "#Nombre:NOMBRE FIRMANTE" + "#Emisor:CODIGO";
        }


        public static ExpedienteDePatente CreateExpediente() 
        {
            var solicitud = new ExpedienteDePatente()
            {
                Expediente = new Expediente(),
                Patente = new Patente(),
                Cronologia = new List<ModelCronologia>(),
                Titulares = new List<ModelTitular>(),
                Inventores = new List<Inventor>(),
                Anualidades = new List<Anualidad>(),
                Prioridades = new List<ModelPrioridad>()
            };
            return solicitud;
        }
    }


    public class ModelPrioridad
    {
        public Prioridad Prioridad { get; set; }
        public string PaisCodigo { get; set; }
    }

}
