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
    
    public partial class Solicitante
    {
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public string Nombre { get; set; }
        public Nullable<int> Edad { get; set; }
        public string EstadoCivil { get; set; }
        public string Profesion { get; set; }
        public string Domicilio { get; set; }
        public string Nacionalidad { get; set; }
        public string LugarNotificacion { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Calidad { get; set; }
        public string EntidadSolicitante { get; set; }
        public string LugarConstitucion { get; set; }
        public string ObjetoSolicitud { get; set; }
        public string EnCalidad { get; set; }
        public string AdquirioDerecho { get; set; }
    }
}
