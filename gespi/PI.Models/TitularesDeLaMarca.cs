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
    
    public partial class TitularesDeLaMarca
    {
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public int TitularId { get; set; }
        public string DireccionParaNotificacion { get; set; }
        public string DireccionParaUbicacion { get; set; }
        public string EnCalidadDe { get; set; }
    }
}
