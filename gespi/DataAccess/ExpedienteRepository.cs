using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using PI.Models.Composite;
using PI.Common;
using System.Data.SqlClient;
using Dapper;
using PI.DataAccess.Scripts;
using PI.Models.Enums;

namespace PI.DataAccess
{
    public class ExpedienteRepository : Repository<Expediente>, IExpedienteRepository
    {
        public ExpedienteRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public string getDOCResol(int cronologiaId)
        {
            var evento = DbContext.CronologiaDoctos.Where(c => c.CronologiaId == cronologiaId).FirstOrDefault();

            return evento == null ? "" : evento.HTMLDOC;
        }

        public Expediente GetBaseDeExpedientePorId(int Id)
        {
            var expediente = (from e in DbContext.Expedientes
                              where e.Id == Id
                              select e).FirstOrDefault();

            return expediente;
        }


        public ExpedienteDeMarca GetExpedienteDeMarcas(string numero)
        {
            // fixes error: A circular reference was detected while serializing an object of type 
            DbContext.Configuration.ProxyCreationEnabled = false;

            var expediente = DbContext.Expedientes
                                .Where(e => e.Numero == numero && e.ModuloId == (int)Modulos.MARCAS)
                                .FirstOrDefault();

            return GetExpedienteDeMarcas(expediente);
        }

        public ExpedienteDeMarca GetExpedienteDeMarcasPorRegistro(int tipoDeRegistroId, int registro, string raya)
        {
            // fixes error: A circular reference was detected while serializing an object of type 
            DbContext.Configuration.ProxyCreationEnabled = false;
            var expediente = new Expediente();
            if (String.IsNullOrEmpty(raya))
            {
                expediente = (from m in DbContext.Marcas
                              join e in DbContext.Expedientes on m.ExpedienteId equals e.Id
                              where m.Registro == registro && e.TipoDeRegistroId == tipoDeRegistroId && e.ModuloId == (int)Modulos.MARCAS
                              select e).Include(m => m.Marcas).FirstOrDefault();
            }
            else {
                expediente = (from m in DbContext.Marcas
                              join e in DbContext.Expedientes on m.ExpedienteId equals e.Id                              
                              where m.Registro == registro && e.TipoDeRegistroId == tipoDeRegistroId && m.Raya == raya && e.ModuloId == (int)Modulos.MARCAS
                              select e).Include(m => m.Marcas).FirstOrDefault();
            }

            return GetExpedienteDeMarcas(expediente);
        }

        public ExpedienteDeMarca GetExpedienteDeMarcasPorId(int Id)
        {
            // fixes error: A circular reference was detected while serializing an object of type 
            DbContext.Configuration.ProxyCreationEnabled = false;

            var expediente = (from m in DbContext.Marcas
                              join e in DbContext.Expedientes on m.ExpedienteId equals e.Id
                              where e.Id == Id && e.ModuloId == (int)Modulos.MARCAS
                              select e).Include(m => m.Marcas).FirstOrDefault();

            return GetExpedienteDeMarcas(expediente);
        }


        private ExpedienteDeMarca GetExpedienteDeMarcas(Expediente expediente)
        {

            if (expediente == null)
                return null;

            DbContext.Entry(expediente).State = EntityState.Detached;

            var expedienteDeMarca = ExpedienteDeMarca.CreateExpediente();

            expedienteDeMarca.Expediente = expediente;

            var marca = DbContext.Marcas.Where(p => p.ExpedienteId == expediente.Id).FirstOrDefault();
            DbContext.Entry(marca).State = EntityState.Detached;
            expedienteDeMarca.Marca = marca;

            expedienteDeMarca.Cronologia = GetCronologia(expediente.Id);

            if (marca.ClassificacionDeNizaId > 0)
            {
                var niza = DbContext.ClassificacionDeNiza.Where(n => n.Id == marca.ClassificacionDeNizaId).FirstOrDefault();
                DbContext.Entry(niza).State = EntityState.Detached;
                expedienteDeMarca.ClassificacionDeNiza = niza;
            }

            var situacionAdmin = DbContext.Estatus.Where(s => s.Id == expediente.EstatusId).FirstOrDefault().Descripcion;
            var tipoDeRegistro = DbContext.TiposDeRegistro.Where(q => q.Id == expediente.TipoDeRegistroId).FirstOrDefault().Nombre;
            var tipoDeMarca = DbContext.TiposDeMarca.Where(q => q.Id == marca.TipoDeMarca).FirstOrDefault().Nombre;
            expedienteDeMarca.SituacionAdmin = situacionAdmin;
            expedienteDeMarca.TipoDeRegistro = tipoDeRegistro;
            expedienteDeMarca.TipoDeMarca = tipoDeMarca;

            expedienteDeMarca.Prioridades = (from p in DbContext.PrioridadMarcas
                                             where p.ExpedienteId == expedienteDeMarca.Expediente.Id
                                             select p).ToList();

            var productos = DbContext.ProductosMarcas.Where(p => p.ExpedienteId == expedienteDeMarca.Expediente.Id).FirstOrDefault();
            if (productos != null)
                DbContext.Entry(productos).State = EntityState.Detached;
            expedienteDeMarca.Productos = productos;

            expedienteDeMarca.Titulares = (from tm in DbContext.TitularesDeLaMarca
                                           join t in DbContext.Titulares on tm.TitularId equals t.Id
                                           where tm.ExpedienteId == expedienteDeMarca.Expediente.Id
                                           select new ModelTitularMarca()
                                           {
                                                Id = tm.Id,
                                                Nombre = t.Nombre,
                                                TitularId = tm.TitularId,
                                                DireccionParaNotificacion = tm.DireccionParaNotificacion,
                                                DireccionParaUbicacion = tm.DireccionParaUbicacion,
                                                EnCalidadDe = tm.EnCalidadDe,
                                                PaisId = t.PaisId
                                           }).ToList();

            expedienteDeMarca.ClasificacionDeViena = (from vm in DbContext.VienaMarcas
                                                      join v in DbContext.ClasificacionDeViena on vm.ClassificacionDeVienaId equals v.Id
                                                      where vm.ExpedienteId == expediente.Id
                                                      select new ModelViena()
                                                      {
                                                          Descripcion = v.Descripcion,
                                                          Codigo = v.Codigo
                                                      }).ToList();
            //TODO: PENDIENTE
            /*
                Renovaciones
                Anotaciones
                 
             */

            return expedienteDeMarca;
        }




        public PagedList BusquedaFonetica(int pageNumber, int pageSize, string textToSearch, string clases) 
        { 
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn.Query<BusquedaExpedienteMarca>(SqlMarca.SQL_BUSQUEDA_FONETICA
                        .Replace("[t-s]", (!string.IsNullOrEmpty(textToSearch) ? textToSearch : ""))
                        .Replace("[clases]", (!string.IsNullOrEmpty(clases) ? "AND niza.Codigo IN ([i])".Replace("[i]", clases) : "")));

                    int totalnRecords = -1;

                    result.DataSet = dataset;
                    result.TotalItems = totalnRecords;
                }

                var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

                result.HasPreviousPage = pageNumber > 1;
                result.HasNextPage = pageNumber < pageCount;
                result.IsFirstPage = pageNumber == 1;
                result.IsLastPage = pageNumber >= pageCount;
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                System.Diagnostics.Trace.TraceError(exception.Message);
            }
            return result;
        }


        public PagedList BusquedaIdentica(int pageNumber, int pageSize, string textToSearch, string clases)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var sql = SqlMarca.SQL_BUSQUEDA_IDENTICA
                        .Replace("[t-s]", (!string.IsNullOrEmpty(textToSearch) ? textToSearch : ""))
                        .Replace("[where]", (!string.IsNullOrEmpty(clases) ? "WHERE niza.Codigo IN ([i])".Replace("[i]", clases) : ""));

                    var dataset = conn.Query<BusquedaExpedienteMarca>(sql);

                    int totalnRecords = -1;

                    result.DataSet = dataset;
                    result.TotalItems = totalnRecords;
                }

                var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

                result.HasPreviousPage = pageNumber > 1;
                result.HasNextPage = pageNumber < pageCount;
                result.IsFirstPage = pageNumber == 1;
                result.IsLastPage = pageNumber >= pageCount;
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                System.Diagnostics.Trace.TraceError(exception.Message);
            }
            return result;
        }




//
// PATENTES
//
        public ExpedienteDePatente GetExpedienteDePatentesPorId(int Id)
        {
            var expediente = (from p in DbContext.Patentes
                              join e in DbContext.Expedientes on p.ExpedienteId equals e.Id
                              where e.Id == Id && e.ModuloId == 2
                              select e).FirstOrDefault();

            return GetExpedienteDePatentes(expediente);
        }

        public ExpedienteDePatente GetExpedienteDePatentesPorRegistro(int tipoDeRegistroId, int registro) 
        {
            var expediente = (from p in DbContext.Patentes
                              join e in DbContext.Expedientes on p.ExpedienteId equals e.Id
                              where p.Registro == registro && e.TipoDeRegistroId == tipoDeRegistroId && e.ModuloId == (int)Modulos.PATENTES
                              select e).FirstOrDefault();

            return GetExpedienteDePatentes(expediente);
        }


        public ExpedienteDePatente GetExpedienteDePatentes(int tipoDeRegistroId, string numero)
        {
            // Load one blogs and its related posts 
            var expediente = DbContext.Expedientes
                                .Where(e => e.Numero == numero && e.TipoDeRegistroId == tipoDeRegistroId && e.ModuloId == (int)Modulos.PATENTES)
                                .FirstOrDefault();

            return GetExpedienteDePatentes(expediente);
        }

        private ExpedienteDePatente GetExpedienteDePatentes(Expediente expediente)
        {
            DbContext.Entry(expediente).State = EntityState.Detached;

            var expedienteDePatente = ExpedienteDePatente.CreateExpediente();

            if (expediente != null)
            {
                var cronologia = GetCronologia(expediente.Id);

                var ipclas = (from ipc in DbContext.IPC
                           where ipc.ExpedienteId == expediente.Id
                           select ipc).ToList();

                var ptitulares = (from pt in DbContext.TitularesDeLaPatente
                                  join t in DbContext.TitularesEnPatentes on pt.TitularId equals t.Id
                                  join p in DbContext.Paises on pt.PaisId equals p.Id
                                  where pt.ExpedienteId == expediente.Id
                                  select new ModelTitular()
                                  {
                                      Nombre = t.Nombre, 
                                      Titular = pt,
                                      PaisCodigo = p.Codigo
                                  }).ToList();

                expedienteDePatente.Expediente = expediente;
                var patente = DbContext.Patentes.Where(p=>p.ExpedienteId==expediente.Id).FirstOrDefault();
                DbContext.Entry(patente).State = EntityState.Detached;
                expedienteDePatente.Patente = patente;

                expedienteDePatente.Cronologia = cronologia;
                expedienteDePatente.Ipc1 = ipclas.Count > 0 ? ipclas.Where(x => x.Indice == 1).First().Classificacion : "";
                expedienteDePatente.Ipc2 = ipclas.Count > 1 ? ipclas.Where(x => x.Indice == 2).First().Classificacion : "";
                expedienteDePatente.Ipc3 = ipclas.Count > 2 ? ipclas.Where(x => x.Indice == 3).First().Classificacion : "";
                expedienteDePatente.Ipc4 = ipclas.Count > 3 ? ipclas.Where(x => x.Indice == 4).First().Classificacion : "";
                expedienteDePatente.Titulares = ptitulares;

                var agente = (from a in DbContext.Agentes
                              where a.Id == patente.AgenteId
                             select a).FirstOrDefault();

                if (agente != null)
                {
                    DbContext.Entry(agente).State = EntityState.Detached;
                    expedienteDePatente.Agente = agente;
                }

                expedienteDePatente.Clasificacion = (from c in DbContext.Clasificaciones
                                                     where c.Id == expedienteDePatente.Patente.ClasificacionId
                                                     select c).FirstOrDefault();
                if (expedienteDePatente.Clasificacion != null)
                    DbContext.Entry(expedienteDePatente.Clasificacion).State = EntityState.Detached;

                expedienteDePatente.Inventores = (from i in DbContext.Inventores
                                                  where i.ExpedienteId == expedienteDePatente.Expediente.Id
                                                  select i).ToList();

                expedienteDePatente.Prioridades = (from pr in DbContext.Prioridades
                                                 join p in DbContext.Paises on pr.PaisId equals p.Id
                                                 where pr.ExpedienteId == expediente.Id
                                                 select new ModelPrioridad()
                                                 {
                                                     Prioridad = pr,
                                                     PaisCodigo = p.Codigo
                                                 }).ToList();

                expedienteDePatente.Anualidades = (from a in DbContext.Anualidades
                                                  where a.ExpedienteId == expedienteDePatente.Expediente.Id
                                                  select a).ToList();
                

                expedienteDePatente.Estatus = DbContext.Estatus.Where(s => s.Id == expediente.EstatusId).FirstOrDefault();
                DbContext.Entry(expedienteDePatente.Estatus).State = EntityState.Detached;
            }

            return expedienteDePatente;
        }

        private const string SQL_BUSQUEDA_PATENTES_DSC = @"
SELECT TOP 500 
	    P.ExpedienteId, 
		e.Numero,
		P.Descripcion, 
		tr.Nombre AS TipoDeRegistro, 
		P.Registro, 
		KEY_TBL.RANK
FROM patentes.Patentes P
INNER JOIN CONTAINSTABLE (patentes.Patentes, Descripcion, '""*[t-s]*""') AS KEY_TBL ON  P.Id = KEY_TBL.[KEY]
INNER JOIN dbo.Expedientes e ON P.ExpedienteId = e.Id
INNER JOIN dbo.TiposDeRegistro tr ON e.TipoDeRegistroId = tr.Id
[where]
ORDER BY KEY_TBL.RANK asc";


        public PagedList BusquedaPatentesDsc(int pageNumber, int pageSize, string textToSearch, int? tipoDeRegistro)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var sql = SQL_BUSQUEDA_PATENTES_DSC
                        .Replace("[t-s]", (!string.IsNullOrEmpty(textToSearch) ? textToSearch : ""))
                        .Replace("[where]", (tipoDeRegistro.HasValue ? "WHERE tr.Id = [i]".Replace("[i]", tipoDeRegistro.ToString()) : ""));

                    var dataset = conn.Query<BusquedaExpedientePatente>(sql);

                    int totalnRecords = -1;

                    result.DataSet = dataset;
                    result.TotalItems = totalnRecords;
                }

                var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

                result.HasPreviousPage = pageNumber > 1;
                result.HasNextPage = pageNumber < pageCount;
                result.IsFirstPage = pageNumber == 1;
                result.IsLastPage = pageNumber >= pageCount;
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                System.Diagnostics.Trace.TraceError(exception.Message);
            }
            return result;
        }



/// <summary>
/// DERECHO DE AUTOR
/// </summary>
        public ExpedienteDAutor GetExpedienteDerechoAutorPorId(int Id)
        {

            var expediente = DbContext.Expedientes
                             .Where(e => e.Id == Id && e.ModuloId == (int)Modulos.DERECHO_DE_AUTOR)
                             .FirstOrDefault();

            return GetExpedienteDerechoAutor(expediente);
        }


        public ExpedienteDAutor GetExpedienteDerechoAutorPorRegistro(int registro)
        {
            var expediente = (from d in DbContext.DerechoDeAutor
                              join e in DbContext.Expedientes on d.ExpedienteId equals e.Id
                              where d.Registro == registro && e.ModuloId == (int)Modulos.DERECHO_DE_AUTOR
                              select e).FirstOrDefault();

            return GetExpedienteDerechoAutor(expediente);
        }

        public ExpedienteDAutor GetExpedienteDerechoAutor(string solicitud)
        {
            // Load one blogs and its related posts 
            var expediente = DbContext.Expedientes
                             .Where(e => e.Numero == solicitud && e.ModuloId == (int)Modulos.DERECHO_DE_AUTOR)
                             .FirstOrDefault();

            return GetExpedienteDerechoAutor(expediente);
        }

        private ExpedienteDAutor GetExpedienteDerechoAutor(Expediente expediente)
        {
            DbContext.Entry(expediente).State = EntityState.Detached;

            var expedienteDAutor = ExpedienteDAutor.CreateExpediente();

            if (expediente != null)
            {
                expedienteDAutor.Expediente = expediente;
                var derechoDeAutor = DbContext.DerechoDeAutor.Where(p=>p.ExpedienteId==expediente.Id).FirstOrDefault();
                if (derechoDeAutor!=null)
                    DbContext.Entry(derechoDeAutor).State = EntityState.Detached;
                expedienteDAutor.DerechoDeAutor = derechoDeAutor;

                var autores = DbContext.Autores.Where(a => a.ExpedienteId == expediente.Id).ToList();
                //DbContext.Entry(autores).State = EntityState.Detached;
                expedienteDAutor.Autores = autores;

                var solicitante = DbContext.Solicitantes.Where(a => a.ExpedienteId == expediente.Id).FirstOrDefault();
                //DbContext.Entry(solicitante).State = EntityState.Detached;
                expedienteDAutor.Solicitante = solicitante;

                var productor = DbContext.Productores.Where(a => a.ExpedienteId == expediente.Id).FirstOrDefault();
                //DbContext.Entry(productor).State = EntityState.Detached;
                expedienteDAutor.Productor = productor;

                var fonoArtistas = DbContext.FonogramaArtistas.Where(a => a.ExpedienteId == expediente.Id).ToList();
                //DbContext.Entry(fonoArtistas).State = EntityState.Detached;
                expedienteDAutor.FonogramaArtistas = fonoArtistas;

                var fonoTituloDeObras = DbContext.FonogramaTituloDeObras.Where(a => a.ExpedienteId == expediente.Id).ToList();
                //DbContext.Entry(fonoTituloDeObras).State = EntityState.Detached;
                expedienteDAutor.FonogramaTituloDeObras = fonoTituloDeObras;

                var literariasyArtisticas = DbContext.LiterariasyArtisticas.Where(a => a.ExpedienteId == expediente.Id).FirstOrDefault();
                //DbContext.Entry(literariasyArtisticas).State = EntityState.Detached;
                expedienteDAutor.LiterariasyArtisticas = literariasyArtisticas;

                var aporteAudiovisual = DbContext.AporteAudiovisual.Where(a => a.ExpedienteId == expediente.Id).FirstOrDefault();
                //DbContext.Entry(aporteAudiovisual).State = EntityState.Detached;
                expedienteDAutor.AporteAudiovisual = aporteAudiovisual;

                var guionAutores = DbContext.GuionAutores.Where(a => a.ExpedienteId == expediente.Id).ToList();
                //DbContext.Entry(guionAutores).State = EntityState.Detached;
                expedienteDAutor.GuionAutores = guionAutores;

                var audiovisualAutores = DbContext.AudiovisualAutores.Where(a => a.ExpedienteId == expediente.Id).ToList();
                //DbContext.Entry(audiovisualAutores).State = EntityState.Detached;
                expedienteDAutor.AudiovisualAutores = audiovisualAutores;

                var composicionAutores = DbContext.ComposicionAutores.Where(a => a.ExpedienteId == expediente.Id).ToList();
                //DbContext.Entry(composicionAutores).State = EntityState.Detached;
                expedienteDAutor.ComposicionAutores = composicionAutores;

                var obrasMusicalesyEscenicas = DbContext.ObrasMusicalesyEscenicas.Where(a => a.ExpedienteId == expediente.Id).FirstOrDefault();
                //DbContext.Entry(obrasMusicalesyEscenicas).State = EntityState.Detached;
                expedienteDAutor.ObrasMusicalesyEscenicas = obrasMusicalesyEscenicas;



                var tipo = DbContext.TiposDeRegistro.Where(p => p.Id == expediente.TipoDeRegistroId).FirstOrDefault();
                DbContext.Entry(tipo).State = EntityState.Detached;
                expedienteDAutor.Tipo = tipo;

                var situacionAdmin = DbContext.Estatus.Where(s => s.Id == expediente.EstatusId).FirstOrDefault().Descripcion;
                expedienteDAutor.SituacionAdmin = situacionAdmin;

                var cronologia = GetCronologia(expediente.Id);
                expedienteDAutor.Cronologia = cronologia;
            }

            return expedienteDAutor;
        }


        private List<ModelCronologia> GetCronologia(int expedienteId)
        {
            var cronologia = (from crono in DbContext.Cronologia
                             join e in DbContext.Estatus on crono.EstatusId equals e.Id
                             where crono.ExpedienteId == expedienteId
                             select new ModelCronologia()
                             {
                                 Id = crono.Id,
                                 Fecha = crono.Fecha,
                                 EstatusId = (int)crono.EstatusId,
                                 EstatusDescripcion = e.Descripcion,
                                 Referencia = crono.Referencia,
                                 UsuarioIniciales = crono.UsuarioIniciales, // should be changed
                                 Observaciones = crono.Observaciones
                             }).ToList();

            return cronologia;
        }

//
// ANOTACIONES
//
        public ExpedienteDeAnotaciones GetExpedienteDeAnotaciones(int tipoDeRegistroId, string numero)
        {
            // Load one blogs and its related posts 
            var expediente = DbContext.Expedientes
                                .Where(e => e.Numero == numero && e.TipoDeRegistroId == tipoDeRegistroId && e.ModuloId == (int)Modulos.ANOTACIONES)
                                .FirstOrDefault();

            return GetExpedienteDeAnotaciones(expediente);
        }

        private ExpedienteDeAnotaciones GetExpedienteDeAnotaciones(Expediente expediente)
        {

            DbContext.Entry(expediente).State = EntityState.Detached;

            var expedienteDeAnotaciones = ExpedienteDeAnotaciones.CreateExpediente();

            if (expediente != null)
            {
                expedienteDeAnotaciones.Expediente = expediente;

                var anotacion = DbContext.Anotaciones.Where(p => p.ExpedienteId == expediente.Id).FirstOrDefault(); //should be always only one

                if (anotacion != null)
                    DbContext.Entry(anotacion).State = EntityState.Detached;

                expedienteDeAnotaciones.Anotacion = anotacion;

                var situacionAdmin = DbContext.Estatus.Where(s => s.Id == expediente.EstatusId).FirstOrDefault();

                if (situacionAdmin != null)
                    expedienteDeAnotaciones.SituacionAdmin = situacionAdmin.Descripcion;

                var cronologia = GetCronologia(expediente.Id);
                expedienteDeAnotaciones.Cronologia = cronologia;

                expedienteDeAnotaciones.ExpedientesConAnotacion = getExpedientesConAnotacion(expediente.Id);
            }

            return expedienteDeAnotaciones;

        }

        //
        // Get expedientes de marcas a anotar parte de la solicitud de anotacion.
        //
        private  List<ModelAnotacionEnExpedientes> getExpedientesConAnotacion(int expedienteId)
        {
            List<ModelAnotacionEnExpedientes> expedientesConAnotacion;

            using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
            {
                var sql = SqlAnotacion.SQL_EXPEDIENTES_DE_MARCA_EN_ANOTACION                    
                    .Replace("[ExpedienteId]", expedienteId.ToString());

                expedientesConAnotacion = conn.Query<ModelAnotacionEnExpedientes>(sql).ToList();

            }

            return expedientesConAnotacion;
        }


        //
        // Save evento cronologico de MARCAS
        //
        public ResultInfo SaveEventoCronologicoDeMarcas(ResolucionCustomizada resolucion)
        {
            if (resolucion.Registrar)
            {
                var sequence = DbContext.Sequence.Where(s => s.KeyName == "ma.Registro").First();
                if (resolucion.Registro == 0)
                {
                    resolucion.Registro = sequence.NextValue;
                    sequence.NextValue++;
                    DbContext.SaveChanges();
                }

                var marca = DbContext.Marcas.Where(e => e.ExpedienteId == resolucion.ExpedienteId).First();
                marca.Tomo = resolucion.Tomo;
                marca.Folio = resolucion.Folio;
                marca.Registro = resolucion.Registro;
                DbContext.SaveChanges();
            }

            return MakeCronologia(resolucion);
        }

        private ResultInfo MakeCronologia(ResolucionCustomizada resolucion)
        {
            // agregar registro en cronologia
            var cronologia = new Cronologia();            
            cronologia.ExpedienteId = resolucion.ExpedienteId;
            cronologia.Fecha = resolucion.Fecha;
            cronologia.EstatusId = resolucion.EstatusId;
            cronologia.Referencia = resolucion.Referencia;
            cronologia.UsuarioId = resolucion.UsuarioId;
            cronologia.Observaciones = resolucion.Observaciones;
            //cronologia.UsuarioIniciales = resolucion.UsuarioId.ToString();
            DbContext.Cronologia.Add(cronologia);
            if (resolucion.EstatusId != resolucion.EstatusFinalId)
            {
                // agregar segundo reg en cronologia
                var cronologia2 = new Cronologia();
                cronologia2.ExpedienteId = resolucion.ExpedienteId;
                cronologia2.Fecha = resolucion.Fecha;
                cronologia2.EstatusId = resolucion.EstatusFinalId;
                cronologia2.Referencia = resolucion.Referencia;
                cronologia2.UsuarioId = resolucion.UsuarioId;
                DbContext.Cronologia.Add(cronologia2);
            }
            if (resolucion.UpdatesEstatus)
            {
                var expediente = DbContext.Expedientes.Where(e => e.Id == resolucion.ExpedienteId).First();
                // cambiar estatus en expediente con estatus EstatusFinalId
                expediente.EstatusId = resolucion.EstatusFinalId;
                expediente.FechaDeEstatus = resolucion.Fecha;
            }
            DbContext.SaveChanges();
            var cronologiaDocto = new CronologiaDocto();
            cronologiaDocto.CronologiaId = cronologia.Id;
            cronologiaDocto.JSONDOC = resolucion.JSONDOC;
            cronologiaDocto.HTMLDOC = resolucion.HTMLDOC;
            cronologiaDocto.FechaActualizacion = DateTime.Now;
            cronologiaDocto.ActualizadoPorUsuarioId = resolucion.UsuarioId;
            cronologiaDocto.EsTitulo = resolucion.esTitulo;
            cronologiaDocto.Llave = resolucion.Llave;
            DbContext.CronologiaDoctos.Add(cronologiaDocto);

            var result = new ResultInfo() { Succeeded = true };
            return result;
        }


        //
        // Save evento cronologico de DERECHO DE AUTOR
        //
        public ResultInfo SaveEventoCronologicoDeDA(ResolucionCustomizada resolucion) 
        {
            if (resolucion.Registrar) 
            {
                var sequence = DbContext.Sequence.Where(s => s.KeyName == "da.Registro").First();
                if (resolucion.Registro == 0) {
                    resolucion.Registro = sequence.NextValue;
                    sequence.NextValue++;
                    DbContext.SaveChanges();
                }

                var da = DbContext.DerechoDeAutor.Where(e => e.ExpedienteId == resolucion.ExpedienteId).First();
                da.Tomo = resolucion.Tomo;
                da.Folio = resolucion.Folio;
                da.Registro = resolucion.Registro;
                da.Libro = resolucion.Libro;
                DbContext.SaveChanges();
            }

            return MakeCronologia(resolucion);
        }


        ///
        /// SaveEventoCronologico DE PATENTES
        ///
        public ResultInfo SaveEventoCronologicoDePatentes(ResolucionCustomizada resolucion)
        {
            var exp = DbContext.Expedientes.Where(e => e.Id == resolucion.ExpedienteId).First();
            if (resolucion.EstatusFinalId == (int)PatenteEstatus.NO_CAMBIA) {
                resolucion.EstatusId = exp.EstatusId;
                resolucion.EstatusFinalId = exp.EstatusId;
            }            

            if (resolucion.Registrar)
            {
                var tipo = string.Empty;
                if (resolucion.Tipo == "10")
                    tipo = "p.A.Numero";
                else if (resolucion.Tipo == "9")
                    tipo = "p.U.Numero";
                else if (resolucion.Tipo == "8")
                    tipo = "p.P.Numero"; // Pendiente
                else if (resolucion.Tipo == "7")
                    tipo = "p.S.Numero";
                else if (resolucion.Tipo == "6")
                    tipo = "p.F.Numero"; // Pendiente

                var sequence = DbContext.Sequence.Where(s => s.KeyName == tipo).First();
                if (resolucion.Registro == 0)
                {
                    resolucion.Registro = sequence.NextValue;
                    sequence.NextValue++;
                    DbContext.SaveChanges();
                }

                var pat = DbContext.Patentes.Where(p => p.ExpedienteId == resolucion.ExpedienteId).First();
                pat.Tomo = resolucion.Tomo;
                pat.Folio = resolucion.Folio;
                pat.Registro = resolucion.Registro;
                //pat.Libro = resolucion.Libro;
                DbContext.SaveChanges();
            }

            return MakeCronologia(resolucion);
        }

    }
}
