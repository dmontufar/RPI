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
    
    public partial class gaceta
    {
        public int Id { get; set; }
        public string Expediente { get; set; }
        public string Marca { get; set; }
        public System.DateTime FechaEdicto { get; set; }
        public System.DateTime FechaPublicacion { get; set; }
        public string Productos { get; set; }
        public string pdfEdicto { get; set; }
        public string Mandatario { get; set; }
    }
}