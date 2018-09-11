using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class ExpedienteManager : Manager<Expediente>, IExpedienteManager
    {
        public ExpedienteManager(IExpedienteRepository repository, ITransaction transaction) : base(repository, transaction) { }

        public ExpedienteDeMarca GetExpedienteDeMarcas(string solicitud)
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDeMarcas(solicitud);

            return expediente;
        }

        public ExpedienteDeMarca GetExpedienteDeMarcasPorRegistro(int tipoDeRegistroId, int registro, string raya) {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDeMarcasPorRegistro(tipoDeRegistroId, registro, raya);

            return expediente;
        }

        public ExpedienteDeMarca GetExpedienteDeMarcasPorId(int Id) 
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDeMarcasPorId(Id);

            return expediente; 
        }

        public ResultInfo BusquedaFonetica(int pageNumber, int pageSize, string textToSearch, string clases)
        {
            var pagedData = ((IExpedienteRepository)_repository)
                .BusquedaFonetica(pageNumber, pageSize, textToSearch, clases);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo BusquedaIdentica(int pageNumber, int pageSize, string textToSearch, string clases)
        {
            var pagedData = ((IExpedienteRepository)_repository)
                .BusquedaIdentica(pageNumber, pageSize, textToSearch, clases);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

/// <summary>
/// MARCAS
/// </summary>
        public ResultInfo SaveEventoCronologicoDeMarcas(ResolucionCustomizada resolucion) 
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IExpedienteRepository)Repository).SaveEventoCronologicoDeMarcas(resolucion);
                return result;
            });
        }
    


/// <summary>
/// PATENTES
/// </summary>
        public ExpedienteDePatente GetExpedienteDePatentes(int tipoDeRegistroId, string solicitud)
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDePatentes(tipoDeRegistroId, solicitud);

            return expediente;
        }

        public ExpedienteDePatente GetExpedienteDePatentesPorRegistro(int tipoDeRegistroId, int registro)
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDePatentesPorRegistro(tipoDeRegistroId, registro);

            return expediente;
        }

        public ExpedienteDePatente GetExpedienteDePatentesPorId(int Id) 
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDePatentesPorId(Id);

            return expediente;
        }

        public ResultInfo BusquedaPatentesDsc(int pageNumber, int pageSize, string textToSearch, int? tipoDeRegistro)
        {
            var pagedData = ((IExpedienteRepository)_repository)
                .BusquedaPatentesDsc(pageNumber, pageSize, textToSearch, tipoDeRegistro);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }


/// <summary>
/// DERECHO DE AUTOR
/// </summary>
        public ExpedienteDAutor GetExpedienteDerechoAutor(string solicitud)
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDerechoAutor(solicitud);

            return expediente;
        }

        public ExpedienteDAutor GetExpedienteDerechoAutorPorRegistro(int registro)
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDerechoAutorPorRegistro(registro);

            return expediente;
        }

        public ExpedienteDAutor GetExpedienteDerechoAutorPorId(int Id) {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDerechoAutorPorId(Id);

            return expediente;
        }


        public ResultInfo SaveEventoCronologicoDeDA(ResolucionCustomizada resolucion)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IExpedienteRepository)Repository).SaveEventoCronologicoDeDA(resolucion);
                return result;
            });   
        }


/// <summary>
/// PATENTES
/// </summary>
        public ResultInfo SaveEventoCronologicoDePatentes(ResolucionCustomizada resolucion) 
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IExpedienteRepository)Repository).SaveEventoCronologicoDePatentes(resolucion);
                return result;
            });
        }


        public ExpedienteDeAnotaciones GetExpedienteDeAnotaciones(int tipoDeRegistroId, string solicitud)
        {
            var expediente = ((IExpedienteRepository)Repository).GetExpedienteDeAnotaciones(tipoDeRegistroId, solicitud);

            return expediente;
        }


///
/// Get Documento electronico
///
        public string getDOCResol(int cronologiaId)
        {
            var result = ((IExpedienteRepository)_repository).getDOCResol(cronologiaId);
            return result;
        }


        public Expediente GetBaseDeExpedientePorId(int Id)
        {
            var result = ((IExpedienteRepository)_repository).GetBaseDeExpedientePorId(Id);
            return result;
        }

    }
}
