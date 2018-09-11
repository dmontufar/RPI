using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Models.Composite;
using PI.Models.Enums;
using System.Data.Entity;

namespace PI.DataAccess
{
    public interface IDerechoDeAutorRepository : IRepository<DerechoDeAutor>
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

    public class DerechoDeAutorRepository : Repository<DerechoDeAutor>, IDerechoDeAutorRepository
    {
        public DerechoDeAutorRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public ResultInfo SaveSolicitud(GenericEntity<ExpedienteDAutor> model) 
        {
            var solicitud = model.Generic as ExpedienteDAutor; //solicitud
            var auditoria = model.Extra as Auditoria;
            if (string.IsNullOrEmpty(auditoria.Historial) || auditoria.Historial == "[]")
                return new ResultInfo() { Succeeded = true, Result = solicitud.Expediente.Id };

            solicitud.Expediente.ActualizadoPorUsuarioId = auditoria.UsuarioId;
            solicitud.Expediente.FechaActualizacion = DateTime.Now;

            if (solicitud.Expediente.Id == -1)
            {
                //Expediente
                solicitud.Expediente.Id = 0;
                solicitud.Expediente.ModuloId = (int)Modulos.DERECHO_DE_AUTOR;
                DbContext.Expedientes.Add(solicitud.Expediente);
                DbContext.SaveChanges();

                //Derecho de Autor
                solicitud.DerechoDeAutor.ExpedienteId = solicitud.Expediente.Id;
                DbContext.DerechoDeAutor.Add(solicitud.DerechoDeAutor);
                DbContext.SaveChanges();

                //AporteAudiovisual
                if (solicitud.AporteAudiovisual != null) 
                {
                    solicitud.AporteAudiovisual.ExpedienteId = solicitud.Expediente.Id;
                    DbContext.AporteAudiovisual.Add(solicitud.AporteAudiovisual);
                }

                //LiterariasyArtisticas
                if (solicitud.LiterariasyArtisticas != null)
                {
                    solicitud.LiterariasyArtisticas.ExpedienteId = solicitud.Expediente.Id;
                    DbContext.LiterariasyArtisticas.Add(solicitud.LiterariasyArtisticas);
                }

                //ObrasMusicalesyEscenicas
                if (solicitud.ObrasMusicalesyEscenicas != null)
                {
                    solicitud.ObrasMusicalesyEscenicas.ExpedienteId = solicitud.Expediente.Id;
                    DbContext.ObrasMusicalesyEscenicas.Add(solicitud.ObrasMusicalesyEscenicas);
                }

                //Productor
                if (solicitud.Productor != null)
                {
                    solicitud.Productor.ExpedienteId = solicitud.Expediente.Id;
                    DbContext.Productores.Add(solicitud.Productor);
                }

                if (solicitud.Solicitante != null)
                {
                    solicitud.Solicitante.ExpedienteId = solicitud.Expediente.Id;
                    DbContext.Solicitantes.Add(solicitud.Solicitante);
                }

                DbContext.SaveChanges();

                var cronologia = new Cronologia(); // footprint separate table
                cronologia.Fecha = solicitud.Cronologia.First().Fecha;
                cronologia.EstatusId = solicitud.Cronologia.First().EstatusId;
                cronologia.UsuarioId = solicitud.Cronologia.First().UsuarioId;
                cronologia.ExpedienteId = solicitud.Expediente.Id;
                DbContext.Cronologia.Add(cronologia);
                DbContext.SaveChanges();
            }
            else
            {
                var expediente = DbContext.Expedientes.Where(e => e.Id == solicitud.Expediente.Id).First();

                expediente.TipoDeRegistroId = solicitud.Expediente.TipoDeRegistroId;
                expediente.FechaDeSolicitud = solicitud.Expediente.FechaDeSolicitud;
                expediente.Hora = solicitud.Expediente.Hora;

                var da = DbContext.DerechoDeAutor.Where(e => e.ExpedienteId == solicitud.Expediente.Id).FirstOrDefault();

                solicitud.DerechoDeAutor.ExpedienteId = solicitud.Expediente.Id;
                if (da == null)
                {
                    DbContext.DerechoDeAutor.Add(solicitud.DerechoDeAutor);
                }
                else 
                {
                    var attachedEntry = DbContext.Entry(da);
                    attachedEntry.CurrentValues.SetValues(solicitud.DerechoDeAutor);
                }
                DbContext.SaveChanges();

                //dAutor.Titulo = solicitud.DerechoDeAutor.Titulo;
                //dAutor.Traduccion = solicitud.DerechoDeAutor.Traduccion;
                //dAutor.Paginas = solicitud.DerechoDeAutor.Paginas;
                //dAutor.Formato = solicitud.DerechoDeAutor.Formato;
                //dAutor.LugarEdicion = solicitud.DerechoDeAutor.LugarEdicion;
                //dAutor.FechaEdicion = solicitud.DerechoDeAutor.FechaEdicion;
                //dAutor.LugarEraPublicacion = solicitud.DerechoDeAutor.LugarEraPublicacion;
                //dAutor.FechaPublicacion = solicitud.DerechoDeAutor.FechaPublicacion;
                //dAutor.Editor = solicitud.DerechoDeAutor.Editor;
                //dAutor.AnioCreacion = solicitud.DerechoDeAutor.AnioCreacion;
                //dAutor.PaisOrigenId = solicitud.DerechoDeAutor.PaisOrigenId;
                //dAutor.EsInedita = solicitud.DerechoDeAutor.EsInedita;
                //dAutor.EsPublicada = solicitud.DerechoDeAutor.EsPublicada;
                //dAutor.EsOriginaria = solicitud.DerechoDeAutor.EsOriginaria;
                //dAutor.EsDerivada = solicitud.DerechoDeAutor.EsDerivada;
                //dAutor.EsIndividual = solicitud.DerechoDeAutor.EsIndividual;
                //dAutor.EsColectiva = solicitud.DerechoDeAutor.EsColectiva;
                //dAutor.EsEnColaboracion = solicitud.DerechoDeAutor.EsEnColaboracion;
                //dAutor.OtraClasificacionAplicable = solicitud.DerechoDeAutor.OtraClasificacionAplicable;
                //dAutor.VersionesAutorizadas = solicitud.DerechoDeAutor.VersionesAutorizadas;
                //dAutor.OtraInfoQueIdentifique = solicitud.DerechoDeAutor.OtraInfoQueIdentifique;
                //dAutor.SoporteMaterial = solicitud.DerechoDeAutor.SoporteMaterial;
                //DbContext.SaveChanges();

                //AporteAudiovisual
                if (solicitud.AporteAudiovisual != null)
                {
                    var aporteAudiovisual = DbContext.AporteAudiovisual.Where(e => e.ExpedienteId == solicitud.Expediente.Id).FirstOrDefault();

                    solicitud.AporteAudiovisual.ExpedienteId = solicitud.Expediente.Id;
                    if (aporteAudiovisual == null)
                    {
                        DbContext.AporteAudiovisual.Add(solicitud.AporteAudiovisual);
                    }
                    else
                    {
                        var attachedEntry = DbContext.Entry(aporteAudiovisual);
                        attachedEntry.CurrentValues.SetValues(solicitud.AporteAudiovisual);
                    }
                    DbContext.SaveChanges();
                }


                //LiterariasyArtisticas
                if (solicitud.LiterariasyArtisticas != null)
                {
                    var literariasyArtisticas = DbContext.LiterariasyArtisticas.Where(e => e.ExpedienteId == solicitud.Expediente.Id).FirstOrDefault();

                    solicitud.LiterariasyArtisticas.ExpedienteId = solicitud.Expediente.Id;
                    if (literariasyArtisticas == null)
                    {
                        DbContext.LiterariasyArtisticas.Add(solicitud.LiterariasyArtisticas);
                    }
                    else
                    {
                        var attachedEntry = DbContext.Entry(literariasyArtisticas);
                        attachedEntry.CurrentValues.SetValues(solicitud.LiterariasyArtisticas);
                    }
                    DbContext.SaveChanges();
                }


                //ObrasMusicalesyEscenicas
                if (solicitud.ObrasMusicalesyEscenicas != null)
                {
                    var obrasMusicalesyEscenicas = DbContext.ObrasMusicalesyEscenicas.Where(e => e.ExpedienteId == solicitud.Expediente.Id).FirstOrDefault();

                    solicitud.ObrasMusicalesyEscenicas.ExpedienteId = solicitud.Expediente.Id;
                    if (obrasMusicalesyEscenicas == null)
                    {
                        DbContext.ObrasMusicalesyEscenicas.Add(solicitud.ObrasMusicalesyEscenicas);
                    }
                    else
                    {
                        var attachedEntry = DbContext.Entry(obrasMusicalesyEscenicas);
                        attachedEntry.CurrentValues.SetValues(solicitud.ObrasMusicalesyEscenicas);
                    }
                    DbContext.SaveChanges();
                }


                //Productor
                if (solicitud.Productor != null)
                {
                    var productor = DbContext.Productores.Where(e => e.ExpedienteId == solicitud.Expediente.Id).FirstOrDefault();

                    solicitud.Productor.ExpedienteId = solicitud.Expediente.Id;
                    if (productor == null)
                    {
                        DbContext.Productores.Add(solicitud.Productor);
                    }
                    else
                    {
                        var attachedEntry = DbContext.Entry(productor);
                        attachedEntry.CurrentValues.SetValues(solicitud.Productor);
                    }
                    DbContext.SaveChanges();
                }

                //Solicitante
                if (solicitud.Solicitante != null)
                {
                    var solicitante = DbContext.Solicitantes.Where(e => e.ExpedienteId == solicitud.Expediente.Id).FirstOrDefault();

                    solicitud.Solicitante.ExpedienteId = solicitud.Expediente.Id;
                    if (solicitante == null)
                    {
                        DbContext.Solicitantes.Add(solicitud.Solicitante);
                    }
                    else
                    {
                        var attachedEntry = DbContext.Entry(solicitante);
                        attachedEntry.CurrentValues.SetValues(solicitud.Solicitante);
                    }
                }
                DbContext.SaveChanges();
            }

            SaveAuditoria(auditoria);

            var result = new ResultInfo() { Succeeded = true, Result = solicitud.Expediente.Id };
            return result;
        }


        /// 
        /// AUTOR
        /// 
        public ResultInfo SaveAutor(GenericEntity<Autor> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var autor = model.Generic as Autor; //Autor data

            if (autor.ExpedienteId <= 0)
                return result;

            DbContext.Autores.Add(autor);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            
            result.Succeeded = true;
            result.Result = autor;
            return result;
        }

        public ResultInfo DeleteAutor(GenericEntity<Autor> model)
        {
            var auditoria = model.Extra as Auditoria;
            var autor = model.Generic as Autor; //Autor data

            var dbAutor = DbContext.Autores.Where(p => p.Id == autor.Id).First();
            DbContext.Autores.Remove(dbAutor);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }


        /// 
        /// FonogramaTituloDeObra
        /// 
        public ResultInfo SaveFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as FonogramaTituloDeObra; // data

            if (_entity.ExpedienteId <= 0)
                return result;

            DbContext.FonogramaTituloDeObras.Add(_entity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            result.Succeeded = true;
            result.Result = _entity;
            return result;
        }

        public ResultInfo DeleteFonogramaTituloDeObra(GenericEntity<FonogramaTituloDeObra> model)
        {
            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as FonogramaTituloDeObra; // data

            var dbEntity = DbContext.FonogramaTituloDeObras.Where(p => p.Id == _entity.Id).First();
            DbContext.FonogramaTituloDeObras.Remove(dbEntity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }

        /// 
        /// FonogramaArtista
        /// 
        public ResultInfo SaveFonogramaArtista(GenericEntity<FonogramaArtista> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as FonogramaArtista; // data

            if (_entity.ExpedienteId <= 0)
                return result;

            DbContext.FonogramaArtistas.Add(_entity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            result.Succeeded = true;
            result.Result = _entity;
            return result;
        }

        public ResultInfo DeleteFonogramaArtista(GenericEntity<FonogramaArtista> model)
        {
            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as FonogramaArtista; // data

            var dbEntity = DbContext.FonogramaArtistas.Where(p => p.Id == _entity.Id).First();
            DbContext.FonogramaArtistas.Remove(dbEntity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }

        //
        //GuionAutor
        //
        public ResultInfo SaveGuionAutor(GenericEntity<GuionAutor> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as GuionAutor; // data

            if (_entity.ExpedienteId <= 0)
                return result;

            DbContext.GuionAutores.Add(_entity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            result.Succeeded = true;
            result.Result = _entity;
            return result;
        }

        public ResultInfo DeleteGuionAutor(GenericEntity<GuionAutor> model)
        {
            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as GuionAutor; // data

            var dbEntity = DbContext.GuionAutores.Where(p => p.Id == _entity.Id).First();
            DbContext.GuionAutores.Remove(dbEntity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }

        //
        //AudiovisualAutor
        //
        public ResultInfo SaveAudiovisualAutor(GenericEntity<AudiovisualAutor> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as AudiovisualAutor; // data

            if (_entity.ExpedienteId <= 0)
                return result;

            DbContext.AudiovisualAutores.Add(_entity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            result.Succeeded = true;
            result.Result = _entity;
            return result;
        }

        public ResultInfo DeleteAudiovisualAutor(GenericEntity<AudiovisualAutor> model)
        {
            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as AudiovisualAutor; // data

            var dbEntity = DbContext.AudiovisualAutores.Where(p => p.Id == _entity.Id).First();
            DbContext.AudiovisualAutores.Remove(dbEntity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }

        //
        //ComposicionAutor
        //
        public ResultInfo SaveComposicionAutor(GenericEntity<ComposicionAutor> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as ComposicionAutor; // data

            if (_entity.ExpedienteId <= 0)
                return result;

            DbContext.ComposicionAutores.Add(_entity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            result.Succeeded = true;
            result.Result = _entity;
            return result;
        }

        public ResultInfo DeleteComposicionAutor(GenericEntity<ComposicionAutor> model)
        {
            var auditoria = model.Extra as Auditoria;
            var _entity = model.Generic as ComposicionAutor; // data

            var dbEntity = DbContext.ComposicionAutores.Where(p => p.Id == _entity.Id).First();
            DbContext.ComposicionAutores.Remove(dbEntity);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }


        public void SaveAuditoria(Auditoria auditoria) 
        {
            DbContext.Auditoria.Add(auditoria);
            DbContext.SaveChanges();
        }

    }
}
