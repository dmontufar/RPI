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
    
    public partial class Prioridad
    {
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public int PaisId { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Tipo_referencia { get; set; }
        public string SolicitudP { get; set; }
    }
}
