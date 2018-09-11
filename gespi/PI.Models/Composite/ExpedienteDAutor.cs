using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class ExpedienteDAutor
    {
        public Expediente Expediente { get; set; }
        public DerechoDeAutor DerechoDeAutor { get; set; } //ok
        public List<ModelCronologia> Cronologia { get; set; } //ok
        public Solicitante Solicitante { get; set; } //ok
        public List<Autor> Autores { get; set; } //ok
        public Productor Productor { get; set; } //ok
        public TiposDeRegistro Tipo { get; set; } //ok

        // Fonogramas
        public List<FonogramaArtista> FonogramaArtistas { get; set; } //ok
        public List<FonogramaTituloDeObra> FonogramaTituloDeObras { get; set; } //ok

        // Literarias - artisticas
        public LiterariasyArtisticas LiterariasyArtisticas { get; set; } //ok

        // Audiovisual - director, genero, clase, metraje, duracion
        public AporteAudiovisual AporteAudiovisual { get; set; } //ok
        public List<GuionAutor> GuionAutores { get; set; } //ok
        public List<AudiovisualAutor> AudiovisualAutores { get; set; } //ok
        public List<ComposicionAutor> ComposicionAutores { get; set; } //ok

        // Obras musicales
        public ObrasMusicalesyEscenicas ObrasMusicalesyEscenicas { get; set; }

        //
        public List<DAResolucion> DAResoluciones { get; set; }
        public string SituacionAdmin { get; set; }  //ok

        public string AutoresCSVString()
        {
            var _autor = "";
            var variosAutores = 0;
            for (var i = 0; i < Autores.Count; i++)
            {
                if (_autor == "")
                {
                    _autor += Autores[i].NombreAutor;
                }
                else
                {
                    variosAutores = 1;
                    if (i + 1 < Autores.Count)
                    {
                        _autor += ", " + Autores[i].NombreAutor;
                    }
                    else
                    {
                        _autor += " Y " + Autores[i].NombreAutor;
                    }
                }
            }
            if (Expediente.TipoDeRegistroId == 13 || Expediente.TipoDeRegistroId == 17)
            {
                _autor = Productor.Nombre;
                _autor = "productor es " + _autor;
            }
            else
            {
                if (variosAutores == 1)
                    _autor = "autores son " + _autor;
                else
                    _autor = "autor es " + _autor;
            }

            return _autor;
        }

        public string getSello(string llave)
        {
            return "#Id:" + llave + "#Exp:" + this.Expediente.Numero + "#Dsc:" + this.DerechoDeAutor.Titulo + "#Fecha:" + this.Expediente.FechaDeEstatus.ToShortDateString() + "#Emisor:YOLODEMO";
        }

        public string getFirma()
        {
            return "#Exp:" + this.Expediente.Numero + "#Nombre:NOMBRE FIRMANTE" + "#Emisor:CODIGO";
        }


        public static ExpedienteDAutor CreateExpediente() 
        {
            var solicitud = new ExpedienteDAutor()
            {
                Expediente = new Expediente(), 
                DerechoDeAutor = new DerechoDeAutor(),
                Cronologia = new List<ModelCronologia>(),
                FonogramaArtistas = new List<FonogramaArtista>(),
                FonogramaTituloDeObras = new List<FonogramaTituloDeObra>(),
                GuionAutores = new List<GuionAutor>(),
                AudiovisualAutores = new List<AudiovisualAutor>(),
                ComposicionAutores = new List<ComposicionAutor>(),
                ObrasMusicalesyEscenicas = new ObrasMusicalesyEscenicas(),
                DAResoluciones = new List<DAResolucion>(),
                Solicitante = new Solicitante(),
                Autores = new List<Autor>(),
                Productor = new Productor()
            };
            return solicitud;
        }
    }
}
