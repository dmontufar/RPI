using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IDatabaseFactory
    {
        GPIEntities MakeDbContext();
        IQueryable<Leyes> GetLeyes();
        IQueryable<Modulo> GetModulos();
        //IQueryable<TipoDeDato> GetTipoDeDatos();
        IQueryable<TiposDeRegistro> GetTiposDePatente();
        IQueryable<TiposDeRegistro> GetTiposDeObra();
        IQueryable<TiposDeRegistro> GetTiposDeRegistroDeMarca();
        IQueryable<TiposDeRegistro> GetTiposDeAnotaciones();
        IQueryable<Pais> GetPaises();
        IQueryable<Clasificacion> GetClasificaciones();
    }
}
