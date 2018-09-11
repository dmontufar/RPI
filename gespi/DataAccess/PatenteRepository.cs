using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Models.Composite;

namespace PI.DataAccess
{
    public interface IPatenteRepository : IRepository<Patente>
    {
        ResultInfo SaveSolicitud(ExpedienteDePatente patente);
        ResultInfo SaveTitulares(GenericEntity<TitularEnPatentes> model, Auditoria auditoria);
        ResultInfo DeleteTitular(int titularId, Auditoria auditoria);
        ResultInfo SaveAgente(GenericEntity<Auditoria> model);
        ResultInfo SaveResumenClasificacion(GenericEntity<Auditoria> model);
        ResultInfo SaveReferencia(GenericEntity<Prioridad> model);
        ResultInfo DeleteReferencia(GenericEntity<Prioridad> model);
        ResultInfo SaveAnualidad(GenericEntity<Anualidad> model);
        ResultInfo DeleteAnualidad(GenericEntity<Anualidad> model);
    }

    public class PatenteRepository : Repository<Patente>, IPatenteRepository
    {
        public PatenteRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public ResultInfo SaveSolicitud(ExpedienteDePatente solicitud) 
        {

            if (solicitud.Expediente.Id == -1)
            {
                solicitud.Expediente.Id = 0;
                solicitud.Patente.FechaRecepcion = solicitud.Expediente.FechaDeEstatus = solicitud.Expediente.FechaDeSolicitud;
                DbContext.Expedientes.Add(solicitud.Expediente);
                DbContext.SaveChanges();
                solicitud.Patente.ExpedienteId = solicitud.Expediente.Id;
                DbContext.Patentes.Add(solicitud.Patente);
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
                var patente = DbContext.Patentes.Where(e => e.ExpedienteId == expediente.Id).First();
                expediente.FechaDeSolicitud = solicitud.Expediente.FechaDeSolicitud;
                expediente.LeyId = solicitud.Expediente.LeyId;
                //expediente.Numero = patente.Expediente.Numero; 
                patente.Descripcion = solicitud.Patente.Descripcion;
                patente.Pct = solicitud.Patente.Pct;
                patente.Fecha_Pct = solicitud.Patente.Fecha_Pct;
                patente.Registro = solicitud.Patente.Registro;
                patente.Folio = solicitud.Patente.Folio;
                patente.Tomo = solicitud.Patente.Tomo;
                patente.ClasificacionId = solicitud.Patente.ClasificacionId;
                DbContext.SaveChanges();
                //solicitud.Expediente = expediente;
                //solicitud.Patente = patente;
            }

            var result = new ResultInfo() { Succeeded = true, Result = solicitud };
            return result;
        }

        public ResultInfo SaveTitulares(GenericEntity<TitularEnPatentes> model, Auditoria auditoria)
        {
            var result = new ResultInfo() { Succeeded = false};

            var patenteTitular = model.Extra as TitularDeLaPatente; //nombre, pais
            if (patenteTitular.ExpedienteId == 0) 
                return result;

            var titular = model.Generic as TitularEnPatentes; //nombre, pais

            if (titular.Id==0)
            {
                DbContext.TitularesEnPatentes.Add(titular);
                DbContext.SaveChanges();
                patenteTitular.TitularId = titular.Id;
            }
            auditoria.Evento += "-"+titular.Id.ToString();
            DbContext.TitularesDeLaPatente.Add(patenteTitular);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            var ptitular = (from pt in DbContext.TitularesDeLaPatente
                            join t in DbContext.TitularesEnPatentes on pt.TitularId equals t.Id
                              join p in DbContext.Paises on pt.PaisId equals p.Id
                              where pt.Id == patenteTitular.Id
                              select new ModelTitular()
                              {
                                  Nombre = t.Nombre,
                                  Titular = pt,
                                  PaisCodigo = p.Codigo
                              }).ToList();
            result.Succeeded = true;
            result.Result = ptitular;
            return result;
        }

        public ResultInfo DeleteTitular(int titularId, Auditoria auditoria)
        {
            var patenteTitular = DbContext.TitularesDeLaPatente.Where(pt => pt.Id == titularId).First();
            DbContext.TitularesDeLaPatente.Remove(patenteTitular);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }


        public ResultInfo SaveAgente(GenericEntity<Auditoria> model) 
        {
            var result = new ResultInfo() { Succeeded = false };
            int expedienteId = model.Extra.ExpedienteId;

            var patente = DbContext.Patentes.Where(e => e.ExpedienteId == expedienteId).FirstOrDefault();
            var auditoria = model.Generic as Auditoria;

            if (patente == null)
                return result;
            patente.AgenteId = model.Extra.AgenteId;
            DbContext.SaveChanges();

            SaveAuditoria(auditoria);
            result.Succeeded = true;
            return result;
        }


        public ResultInfo SaveResumenClasificacion(GenericEntity<Auditoria> model)
        {
            var result = new ResultInfo() { Succeeded = false };
            int expedienteId = model.Extra.ExpedienteId;
            string Ipc1 = model.Extra.Ipc1;
            string Ipc2 = model.Extra.Ipc2;
            string Ipc3 = model.Extra.Ipc3;
            string Ipc4 = model.Extra.Ipc4;

            var patente = DbContext.Patentes.Where(e => e.ExpedienteId == expedienteId).FirstOrDefault();
            var resumen = model.Extra.Resumen;
            var extra = string.Empty;
            if (!patente.Resumen.Equals(resumen)){
                extra += "Resumen : '" + resumen + "',";
                patente.Resumen = resumen;
            }

            var ipclas = (from ipc in DbContext.IPC
                          where ipc.ExpedienteId == expedienteId
                          select ipc);
            var ipCount = ipclas.Count();
            DbContext.SaveChanges();
            var dbIpc1 = new IPC() { ExpedienteId = expedienteId, Indice = 1 };
            if (ipCount > 0)
            {
                dbIpc1 = ipclas.Where(c => c.Indice == 1).First();
                if (!dbIpc1.Classificacion.Equals(Ipc1)) {
                    extra += "IPC1 : '" + Ipc1 + "',";
                    dbIpc1.Classificacion = Ipc1;
                }
            }

            var dbIpc2 = new IPC() { ExpedienteId = expedienteId, Indice = 2 };
            if (ipCount > 1)
            {
                dbIpc2 = ipclas.Where(c => c.Indice == 2).First();
                if (!dbIpc2.Classificacion.Equals(Ipc2))
                {
                    extra += "IPC2 : '" + Ipc2 + "',";
                    dbIpc2.Classificacion = Ipc2;
                }
            }

            var dbIpc3 = new IPC() { ExpedienteId = expedienteId, Indice = 3 };
            if (ipCount > 2)
            {
                dbIpc3 = ipclas.Where(c => c.Indice == 3).First();
                if (!dbIpc3.Classificacion.Equals(Ipc3))
                {
                    extra += "IPC3 : '" + Ipc3 + "',";
                    dbIpc3.Classificacion = Ipc3;
                }
            }

            var dbIpc4 = new IPC() { ExpedienteId = expedienteId, Indice = 4 };
            if (ipCount > 3)
            {
                dbIpc4 = ipclas.Where(c => c.Indice == 4).First();
                if (!dbIpc4.Classificacion.Equals(Ipc4))
                {
                    extra += "IPC4 : '" + Ipc4 + "',";
                    dbIpc4.Classificacion = Ipc4;
                }
            }
            var auditoria = model.Generic as Auditoria;
            auditoria.Historial = extra;

            DbContext.SaveChanges();
            result.Succeeded = true;

            return result;
        }

        /// 
        /// PRIORIDADES
        /// 
        public ResultInfo SaveReferencia(GenericEntity<Prioridad> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var prioridad = model.Generic as Prioridad; //prioridad data

            if (prioridad.ExpedienteId <= 0)
                return result;


            DbContext.Prioridades.Add(prioridad);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            var referencia = (from pr in DbContext.Prioridades
                          join p in DbContext.Paises on pr.PaisId equals p.Id
                          where pr.Id == prioridad.Id
                          select new ModelPrioridad()
                          {
                            Prioridad = pr,
                            PaisCodigo = p.Codigo
                          }).First();

            result.Succeeded = true;
            result.Result = referencia;
            return result;
        }

        public ResultInfo DeleteReferencia(GenericEntity<Prioridad> model)
        {
            var auditoria = model.Extra as Auditoria;
            var prioridad = model.Generic as Prioridad; //prioridad data

            var referencia = DbContext.Prioridades.Where(p => p.Id == prioridad.Id).First();
            DbContext.Prioridades.Remove(referencia);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);
            var result = new ResultInfo() { Succeeded = true };
            return result;
        }


        public ResultInfo SaveAnualidad(GenericEntity<Anualidad> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            var auditoria = model.Extra as Auditoria;
            var anualidad = model.Generic as Anualidad; //Anualidad data

            if (anualidad.ExpedienteId <= 0)
                return result;


            DbContext.Anualidades.Add(anualidad);
            DbContext.SaveChanges();
            SaveAuditoria(auditoria);

            result.Succeeded = true;
            result.Result = anualidad;
            return result;
        }

        public ResultInfo DeleteAnualidad(GenericEntity<Anualidad> model)
        {
            var auditoria = model.Extra as Auditoria;
            var anualidad = model.Generic as Anualidad; //Anualidad data

            var referencia = DbContext.Anualidades.Where(p => p.Id == anualidad.Id).First();
            DbContext.Anualidades.Remove(referencia);
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
