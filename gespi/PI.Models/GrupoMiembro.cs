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
    
    public partial class GrupoMiembro
    {
        public int Id { get; set; }
        public int GrupoId { get; set; }
        public int UsuarioId { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}