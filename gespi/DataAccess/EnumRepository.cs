using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public class EnumRepository : IEnumRepository
    {
        private IDatabaseFactory _enumFactory;
        public EnumRepository(IDatabaseFactory dbFactory) 
        {
            _enumFactory = dbFactory;
        }

        public IEnumerable<Leyes> GetLeyes()
        {
            return _enumFactory.GetLeyes().ToList();
        }

        public IEnumerable<Modulo> GetModulos()
        {
            return _enumFactory.GetModulos().ToList();
        }

        //public IEnumerable<TipoDeDato> GetTipoDeDatos()
        //{
        //    return _enumFactory.GetTipoDeDatos().ToList();
        //}

        public IEnumerable<TiposDeRegistro> GetTiposDePatente()
        {
            return _enumFactory.GetTiposDePatente().ToList();
        }

        public IEnumerable<TiposDeRegistro> GetTiposDeObra()
        {
            return _enumFactory.GetTiposDeObra().ToList();
        }

        public IEnumerable<TiposDeRegistro> GetTiposDeRegistroDeMarca()
        {
            return _enumFactory.GetTiposDeRegistroDeMarca().ToList();
        }

        public IEnumerable<TiposDeRegistro> GetTiposDeAnotaciones()
        {
            return _enumFactory.GetTiposDeAnotaciones().ToList();
        }


        public IEnumerable<Pais> GetPaises()
        {
            return _enumFactory.GetPaises().ToList();
        }

        public IEnumerable<Clasificacion> GetClasificaciones()
        {
            return _enumFactory.GetClasificaciones().ToList();
        }


    }
}
