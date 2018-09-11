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
    public interface IDerechoDeAutorManager : IManager<DerechoDeAutor>
    {
        ResultInfo SaveSolicitud(GenericEntity<ExpedienteDAutor> model);
        ResultInfo SaveAutor(GenericEntity<Autor> model);
        ResultInfo DeleteAutor(GenericEntity<Autor> model);
        ResultInfo SaveFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model);
        ResultInfo DeleteFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model);
        ResultInfo SaveFonogramaArtista(GenericEntity<FonogramaArtista> model);
        ResultInfo DeleteFonogramaArtista(GenericEntity<FonogramaArtista> model);

        ResultInfo SaveGuionAutor(GenericEntity<GuionAutor> model);
        ResultInfo DeleteGuionAutor(GenericEntity<GuionAutor> model);

        ResultInfo SaveAudiovisualAutor(GenericEntity<AudiovisualAutor> model);
        ResultInfo DeleteAudiovisualAutor(GenericEntity<AudiovisualAutor> model);

        ResultInfo SaveComposicionAutor(GenericEntity<ComposicionAutor> model);
        ResultInfo DeleteComposicionAutor(GenericEntity<ComposicionAutor> model);
    }

    public class DerechoDeAutorManager : Manager<DerechoDeAutor>, IDerechoDeAutorManager
    {
        public DerechoDeAutorManager(IDerechoDeAutorRepository repository, ITransaction transaction) : base(repository, transaction) { }


        public ResultInfo SaveSolicitud(GenericEntity<ExpedienteDAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveSolicitud(model);
                return result;
            });            
        }

        public ResultInfo SaveAutor(GenericEntity<Autor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveAutor(model);
                return result;
            });
        }


        public ResultInfo DeleteAutor(GenericEntity<Autor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).DeleteAutor(model);
                return result;
            });
        }

        public ResultInfo SaveFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveFonogramaTituloDeObra(model);
                return result;
            });
        }

        public ResultInfo DeleteFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).DeleteFonogramaTituloDeObra(model);
                return result;
            });
        }

        public ResultInfo SaveFonogramaArtista(GenericEntity<FonogramaArtista> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveFonogramaArtista(model);
                return result;
            });
        }

        public ResultInfo DeleteFonogramaArtista(GenericEntity<FonogramaArtista> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).DeleteFonogramaArtista(model);
                return result;
            });
        }

        //
        //
        //
        public ResultInfo SaveGuionAutor(GenericEntity<GuionAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveGuionAutor(model);
                return result;
            });
        }

        public ResultInfo DeleteGuionAutor(GenericEntity<GuionAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).DeleteGuionAutor(model);
                return result;
            });
        }

        //
        //
        //
        public ResultInfo SaveAudiovisualAutor(GenericEntity<AudiovisualAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveAudiovisualAutor(model);
                return result;
            });
        }

        public ResultInfo DeleteAudiovisualAutor(GenericEntity<AudiovisualAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).DeleteAudiovisualAutor(model);
                return result;
            });
        }

        //
        //
        //
        public ResultInfo SaveComposicionAutor(GenericEntity<ComposicionAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).SaveComposicionAutor(model);
                return result;
            });
        }

        public ResultInfo DeleteComposicionAutor(GenericEntity<ComposicionAutor> model)
        {
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                var result = ((IDerechoDeAutorRepository)Repository).DeleteComposicionAutor(model);
                return result;
            });
        }



    }
}
