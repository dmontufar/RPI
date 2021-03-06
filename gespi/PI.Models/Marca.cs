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
    
    public partial class Marca
    {
        public Marca()
        {
            this.LogosMarcas = new HashSet<LogosMarcas>();
            this.ProductosMarcas = new HashSet<ProductosMarca>();
            this.VienaMarcas = new HashSet<VienaMarcas>();
        }
    
        public int Id { get; set; }
        public int ExpedienteId { get; set; }
        public string Recibo { get; set; }
        public int TipoDeMarca { get; set; }
        public string Denominacion { get; set; }
        public string Traduccion { get; set; }
        public bool Industrial { get; set; }
        public bool DeServicios { get; set; }
        public bool Comercial { get; set; }
        public bool Certificacion { get; set; }
        public bool Colectiva { get; set; }
        public Nullable<int> Registro { get; set; }
        public string Raya { get; set; }
        public Nullable<decimal> Tomo { get; set; }
        public Nullable<decimal> Folio { get; set; }
        public Nullable<int> ClassificacionDeNizaId { get; set; }
        public string Productos { get; set; }
        public string Reservas { get; set; }
        public string DescripcionGrafica { get; set; }
        public string DoctosAdjuntos { get; set; }
        public string OtrosDoctosAdjuntos { get; set; }
        public string CaracteristicasCom { get; set; }
        public string EstandaresDeCalidad { get; set; }
        public string AutoridadApReglamento { get; set; }
        public string DireccionComercializacion { get; set; }
        public string UltimaRenovacion { get; set; }
        public string ExtensionDeLaMarca { get; set; }
        public Nullable<int> PaisConstituidaId { get; set; }
        public string UbicacionActual { get; set; }
        public string UbicacionAnterior { get; set; }
        public Nullable<System.DateTime> FechaDeTraslado { get; set; }
        public string MotivoDeTraslado { get; set; }
    
        public virtual ClassificacionDeNiza ClassificacionDeNiza { get; set; }
        public virtual Expediente Expedientes { get; set; }
        public virtual ICollection<LogosMarcas> LogosMarcas { get; set; }
        public virtual ICollection<ProductosMarca> ProductosMarcas { get; set; }
        public virtual ICollection<VienaMarcas> VienaMarcas { get; set; }
    }
}
