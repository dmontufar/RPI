using PI.Core;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PI.Internal.Models
{
    public class EnumModels
    {
        private static IEnumManager _enumManager;
        //private static IList<TipoDeDato> _tiposDeData;
        private static IList<Modulo> _modulos;
        private static IList<Leyes> _leyes;
        private static IList<TiposDeRegistro> _tiposDePatente;
        private static IList<TiposDeRegistro> _tiposDeObra;
        private static IList<TiposDeRegistro> _tiposDeRegistroDeMarca;
        private static IList<TiposDeRegistro> _tiposDeAnotaciones;
        private static IList<Pais> _Paises;
        private static IList<Clasificacion> _clasificaciones;

        static EnumModels()
        { 
            _enumManager = new EnumManager(new EnumRepository(new DatabaseFactory()));
            //_tiposDeData = _enumManager.GetTipoDeDatos().Result as IList<TipoDeDato>;
            _modulos = _enumManager.GetModulos().Result as IList<Modulo>;
            _leyes = _enumManager.GetModulos().Result as IList<Leyes>;
            _tiposDePatente = _enumManager.GetTiposDePatente().Result as IList<TiposDeRegistro>;
            _tiposDeObra = _enumManager.GetTiposDeObra().Result as IList<TiposDeRegistro>;
            _tiposDeRegistroDeMarca = _enumManager.GetTiposDeRegistroDeMarca().Result as IList<TiposDeRegistro>;
            _tiposDeAnotaciones = _enumManager.GetTiposDeAnotaciones().Result as IList<TiposDeRegistro>;
            _Paises = _enumManager.GetPaises().Result as IList<Pais>;
            _clasificaciones = _enumManager.GetClasificaciones().Result as IList<Clasificacion>;
        }

        public static IList<Modulo> GetModulos()
        {
            return _modulos;
        }

        public static IList<TiposDeRegistro> GetTiposDePatente()
        {
            return _tiposDePatente;
        }

        public static IList<TiposDeRegistro> GetTiposDeObra()
        {
            return _tiposDeObra;
        }

        public static IList<TiposDeRegistro> GetTiposDeRegistroDeMarca()
        {
            return _tiposDeRegistroDeMarca;
        }

        public static IList<TiposDeRegistro> GetTiposDeAnotaciones()
        {
            return _tiposDeAnotaciones;
        }

        public static IList<Leyes> GetLeyes()
        {
            return _leyes;
        }

        public static IList<Pais> GetPaises()
        {
            return _Paises;
        }

        public static IList<Clasificacion> GetClasificaciones()
        {
            return _clasificaciones;
        }

    }
}