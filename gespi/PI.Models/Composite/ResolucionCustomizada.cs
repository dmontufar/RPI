using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{

    public class ResolucionCustomizada
    {
        public int ExpedienteId { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Articulos { get; set; }
        public string Omitio { get; set; }
        public string Referente { get; set; }
        public int Tomo { get; set; }
        public int Folio { get; set; }
        public int? Registro { get; set; }
        public string Libro { get; set; }
        public dynamic DOCUMENT { get; set; }
        public string JSONDOC { get; set; }
        public string HTMLDOC { get; set; }
        public int EstatusId { get; set; } // Enum
        public string Referencia { get; set; }
        public int UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public bool UpdatesEstatus { get; set; } // flag to let the repository to update estatus del expediente...
        public int EstatusFinalId { get; set; } // if EstatusFinalId != EstatusId se crea otro registro en cronologia y se actualiza el estatus del expediente
        public bool Registrar { get; set; }
        public bool IsRepo { get; set; }
        public string Tipo { get; set; } //Tipo de patente
        public string Titulo { get; set; }
        public bool esTitulo { get; set; }
        public Nullable<System.Guid> Llave { get; set; }
    }

}
