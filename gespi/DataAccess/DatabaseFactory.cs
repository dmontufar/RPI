using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private GPIEntities _entities;

        public GPIEntities MakeDbContext()
        {
            return _entities ?? (_entities = new GPIEntities());
        }

        public IQueryable<Leyes> GetLeyes()
        {
            return MakeDbContext().Leyes;
        }

        public IQueryable<Modulo> GetModulos()
        {
            return MakeDbContext().Modulos;
        }

        //public IQueryable<TipoDeDato> GetTipoDeDatos()
        //{
        //    return MakeDbContext().TipoDeDatos;
        //}

        public IQueryable<TiposDeRegistro> GetTiposDePatente()
        {
            return MakeDbContext().TiposDeRegistro.Where(t=>t.ModuloId==2);
        }

        public IQueryable<TiposDeRegistro> GetTiposDeObra()
        {
            return MakeDbContext().TiposDeRegistro.Where(t => t.ModuloId == 3);
        }

        public IQueryable<TiposDeRegistro> GetTiposDeRegistroDeMarca()
        {
            return MakeDbContext().TiposDeRegistro.Where(t => t.ModuloId == 1);
        }

        public IQueryable<TiposDeRegistro> GetTiposDeAnotaciones()
        {
            return MakeDbContext().TiposDeRegistro.Where(t => t.ModuloId == 4);
        }

        public IQueryable<Pais> GetPaises()
        {
            return MakeDbContext().Paises;
        }

        public IQueryable<Clasificacion> GetClasificaciones()
        { 
            return MakeDbContext().Clasificaciones.OrderBy(o=>o.Descripcion);
        }
    }
}
