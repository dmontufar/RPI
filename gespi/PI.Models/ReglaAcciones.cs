//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReglaAcciones
    {
        public int Id { get; set; }
        public int ReglaId { get; set; }
        public string Condicion { get; set; }
        public int EstatusId { get; set; }
        public bool EnviarCorreoAlSolicitante { get; set; }
        public string Mensaje { get; set; }
        public string EjecutarProceso { get; set; }
    
        public virtual Regla Reglas { get; set; }
    }
}
