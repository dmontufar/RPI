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
    
    public partial class Acciones
    {
        public int Id { get; set; }
        public int ReglaId { get; set; }
        public int ExpedienteId { get; set; }
        public string METADATA { get; set; }
        public Nullable<System.DateTime> FechaDeActualizacion { get; set; }
        public int ActualizadoPorUsuarioId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public bool Draft { get; set; }
        public int EstatusId { get; set; }
    
        public virtual Expediente Expedientes { get; set; }
        public virtual Regla Reglas { get; set; }
    }
}
