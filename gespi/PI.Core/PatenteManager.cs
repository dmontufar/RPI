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
    public interface IPatenteManager : IManager<Patente>
    {
        ResultInfo SaveSolicitud(ExpedienteDePatente solicitud);
        ResultInfo SaveTitulares(GenericEntity<TitularEnPatentes> model, Auditoria auditoria);
        ResultInfo DeleteTitular(int titularId, Auditoria auditoria);
        ResultInfo SaveAgente(GenericEntity<Auditoria> model);
        ResultInfo SaveResumenClasificacion(GenericEntity<Auditoria> model);
        ResultInfo SaveReferencia(GenericEntity<Prioridad> model);
        ResultInfo DeleteReferencia(GenericEntity<Prioridad> model);
        ResultInfo SaveAnualidad(GenericEntity<Anualidad> model);
        ResultInfo DeleteAnualidad(GenericEntity<Anualidad> model);
    }

    public class PatenteManager : Manager<Patente>, IPatenteManager
    {
        public PatenteManager(IPatenteRepository repository, ITransaction transaction) : base(repository, transaction) { }


        public ResultInfo SaveSolicitud(ExpedienteDePatente solicitud)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).SaveSolicitud(solicitud);
                return result;
            });            
        }

        public ResultInfo SaveTitulares(GenericEntity<TitularEnPatentes> model, Auditoria auditoria) 
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).SaveTitulares(model, auditoria);
                return result;
            });            
        }

        public ResultInfo DeleteTitular(int titularId, Auditoria auditoria)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).DeleteTitular(titularId, auditoria);
                return result;
            });
        }

        public ResultInfo SaveAgente(GenericEntity<Auditoria> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).SaveAgente(model);
                return result;
            });
        }

        public ResultInfo SaveResumenClasificacion(GenericEntity<Auditoria> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).SaveResumenClasificacion(model);
                return result;
            });
        }

        public ResultInfo SaveReferencia(GenericEntity<Prioridad> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).SaveReferencia(model);
                return result;
            });
        }

        public ResultInfo DeleteReferencia(GenericEntity<Prioridad> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).DeleteReferencia(model);
                return result;
            });
        }

        public ResultInfo SaveAnualidad(GenericEntity<Anualidad> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).SaveAnualidad(model);
                return result;
            });
        }

        public ResultInfo DeleteAnualidad(GenericEntity<Anualidad> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IPatenteRepository)Repository).DeleteAnualidad(model);
                return result;
            });
        }

    }
}
