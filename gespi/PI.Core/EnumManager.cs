using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class EnumManager : IEnumManager 
    {
        private IEnumRepository _repository;
        private ResultInfo _result;

        public EnumManager(IEnumRepository repository)
        {
            _repository = repository;
            _result = new ResultInfo();
        }

        public ResultInfo GetLeyes()
        {
            var leyes = _repository.GetLeyes();
            return new ResultInfo() { Succeeded = true, Result = leyes };
        }

        public ResultInfo GetModulos()
        {
            var modulos = _repository.GetModulos();
            return new ResultInfo() { Succeeded = true, Result = modulos }; 
        }

        //public ResultInfo GetTipoDeDatos()
        //{
        //    var result = _repository.GetTipoDeDatos();
        //    return new ResultInfo() { Succeeded = true, Result = result };
        //}

        public ResultInfo GetTiposDePatente()
        {
            var result = _repository.GetTiposDePatente();
            return new ResultInfo() { Succeeded = true, Result = result };
        }

        public ResultInfo GetTiposDeObra()
        {
            var result = _repository.GetTiposDeObra();
            return new ResultInfo() { Succeeded = true, Result = result };
        }

        public ResultInfo GetTiposDeRegistroDeMarca()
        {
            var result = _repository.GetTiposDeRegistroDeMarca();
            return new ResultInfo() { Succeeded = true, Result = result };
        }

        public ResultInfo GetTiposDeAnotaciones()
        {
            var result = _repository.GetTiposDeAnotaciones();
            return new ResultInfo() { Succeeded = true, Result = result };
        }


        public ResultInfo GetPaises()
        {
            var result = _repository.GetPaises();
            return new ResultInfo() { Succeeded = true, Result = result };
        }

        public ResultInfo GetClasificaciones()
        {
            var result = _repository.GetClasificaciones();
            return new ResultInfo() { Succeeded = true, Result = result };
        }

    }
}
