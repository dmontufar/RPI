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
    
    public partial class Cronologia
    {
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public System.DateTime Fecha { get; set; }
        public Nullable<int> EstatusId { get; set; }
        public string Referencia { get; set; }
        public Nullable<int> UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public string UsuarioIniciales { get; set; }
        public string JSONDOC { get; set; }
        public string HTMLDOC { get; set; }
    
        public virtual Expediente Expedientes { get; set; }
    }
}
