using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PI.Models.Composite;

namespace PI.DataAccess
{
    public class LeyesRepository : Repository<Leyes>, ILeyesRepository
    {
        public LeyesRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class PermisoRepository : Repository<Permiso>, IPermisoRepository
    {
        private IAuditoriaRepository _auditoriaRepository;

        public PermisoRepository(IDatabaseFactory dbFactory, IAuditoriaRepository auditoriaRepository) : base(dbFactory) 
        { 
            _auditoriaRepository = auditoriaRepository;
        }

        public ResultInfo SaveAll(List<Permiso> model)
        {
            var result = new ResultInfo() { Succeeded = false };

            if (model.Count == 0)
                return result;

            var usuarioId = model[0].UsuarioId;
            var otorga = model[0].OtorgadoPorUsuarioId;

            var dbPerms = DbContext.Permisos.Where(p => p.UsuarioId == usuarioId).ToList();

            if (dbPerms.Count == 0) // insert all
            {
                foreach (var p in model) 
                {
                    p.OtorgadoPorUsuarioId = otorga;
                    DbContext.Permisos.Add(p);
                }
            }
            else  // update
            {
                foreach (var p in model)
                {
                    var dbPerm = dbPerms.Where(dbp => dbp.Opcion == p.Opcion).FirstOrDefault();
                    if (dbPerm != null)
                    {
                        dbPerm.OtorgadoPorUsuarioId = otorga;
                        dbPerm.Acceso = p.Acceso;
                    }
                    else 
                    {
                        p.OtorgadoPorUsuarioId = otorga;
                        DbContext.Permisos.Add(p);
                    }
                }
            }
            DbContext.SaveChanges();
            var auditoria = new Auditoria() 
            {
                Evento = "Permisos",
                Fecha = DateTime.Now,
                ExpedienteId = 0,
                Historial = "UsuarioId:"+ usuarioId,
                UsuarioId = otorga
            };

            _auditoriaRepository.Add(auditoria);

            result.Succeeded = true;
            
            return result;

        }
    }

    public class RolRepository : Repository<Rol>, IRolRepository
    {
        public RolRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class PublicacionRepository : Repository<Publicacion>, IPublicacionRepository
    {
        public PublicacionRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class CronologiaRepository : Repository<Cronologia>, ICronologiaRepository
    {
        public CronologiaRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class FavoritoRepository : Repository<Favorito>, IFavoritoRepository 
    { 
        public FavoritoRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public override Favorito Add(Favorito entity)
        {
            throw new Exception("This operation is disabled.");
        }

        public Favorito Add(Favorito entity, int grupoId)
        {
            if (grupoId == 0)
                grupoId = DbContext.Grupos.Count(g => g.OwnerId == entity.UsuarioId);

            if (grupoId == 0)
            {
                // create first grupo
                var grupo = new Grupo() { OwnerId = entity.UsuarioId, Nombre = "Todos", CreatedDate = DateTime.Now };
                DbContext.Grupos.Add(grupo);
                DbContext.SaveChanges();

                //grupoId = DbContext.Grupos.Where(g => g.OwnerId == entity.UsuarioId && g.Nombre == "Todos").FirstOrDefault().Id;
                grupoId = grupo.Id;

                // agregar el usuario como miembro del grupo
                var miembro = new GrupoMiembro() { GrupoId = grupoId, UsuarioId = entity.UsuarioId, CreatedDate = DateTime.Now };
                DbContext.GrupoMiembros.Add(miembro);
                DbContext.SaveChanges();
            }

            var dbEntity = DbContext.Favoritos.Where(f=>f.ExpedienteId==entity.ExpedienteId).FirstOrDefault();
            if (dbEntity == null)
            {
                DbContext.Favoritos.Add(entity);
                DbContext.SaveChanges();            
            }

            // agregar el expediente al grupo
            if (!DbContext.GrupoExpedientes.Where(ge => ge.FavoritoId == entity.Id && ge.GrupoId == grupoId).Any())
            {
                var grupoExp = new GrupoExpediente() { FavoritoId = entity.Id, GrupoId = grupoId };
                DbContext.GrupoExpedientes.Add(grupoExp);
                DbContext.SaveChanges();
            }
            return entity;
        }

        private const string GET_FAVORITOS_POR_GRUPO = @"
        WITH ctsFavoritos AS
        (
            SELECT f.ExpedienteId, e.Numero, f.Notas, t.Nombre AS TipoDeRegistro, e.FechaDeSolicitud, s.Descripcion AS EstatusDsc, e.FechaDeEstatus
			    , CASE WHEN e.ModuloId = 1 THEN m.Denominacion ELSE CASE WHEN e.ModuloId = 2 THEN p.Descripcion ELSE da.Titulo END END AS Titulo
			    , e.ModuloId
                , ROW_NUMBER() OVER (ORDER BY Numero) AS rownumber
            FROM [ip_external].[GrupoExpedientes] ge
            INNER JOIN [ip_external].[Favoritos] f ON ge.FavoritoId = f.Id
            INNER JOIN dbo.Expedientes e ON f.ExpedienteId = e.Id
            INNER JOIN [ip_external].[GrupoMiembros] gm ON gm.GrupoId = ge.GrupoId
            INNER JOIN [ip_external].[Grupos] g ON ge.GrupoId = g.Id
            INNER JOIN dbo.TiposDeRegistro t ON e.TipoDeRegistroId = t.Id
            INNER JOIN dbo.Estatus s ON e.EstatusId = s.Id
            LEFT JOIN dbo.Marcas m ON e.Id = m.ExpedienteId
            LEFT JOIN patentes.Patentes p ON e.Id = p.ExpedienteId
            LEFT JOIN da.DerechoDeAutor da ON e.Id = da.ExpedienteId
            WHERE f.UsuarioId = @usuarioId [!Grupo!]
        )
        SELECT *
        FROM ctsFavoritos
        WHERE rownumber > (@page -1)*@pagesize AND rownumber <= (@page-1)*@pagesize + @pagesize
        ";

        private const string GET_TOTAL_FAVORITOS = @"
        SELECT Count(*)
        FROM [ip_external].[GrupoExpedientes] ge
        INNER JOIN [ip_external].Favoritos f ON ge.FavoritoId = f.Id
        WHERE f.UsuarioId = @usuarioId [!Grupo!]
        ";

        public PagedList GetPage(int pageNumber, int pageSize, int? idGrupo, int idUsuario)
        {
            
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var favoritos = conn.Query<MiFavorito>(
                        GET_FAVORITOS_POR_GRUPO.Replace("[!Grupo!]", (idGrupo.HasValue ? "AND ge.GrupoId = @grupoId" : "")),
                        new { page = pageNumber, pagesize = pageSize, usuarioId = idUsuario, grupoId = idGrupo });

                    int totalnRecords = conn.Query<int>(GET_TOTAL_FAVORITOS.Replace("[!Grupo!]", (idGrupo.HasValue ? "AND ge.GrupoId = @grupoId" : "")), new { usuarioId = idUsuario, grupoId = idGrupo }).Single();

                    result.DataSet = favoritos;
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
    }

    public class GrupoRepository : Repository<Grupo>, IGrupoRepository 
    { 
        public GrupoRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class GrupoExpedienteRepository : Repository<GrupoExpediente>, IGrupoExpedienteRepository 
    { 
        public GrupoExpedienteRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    public class GrupoMiembroRepository : Repository<GrupoMiembro>, IGrupoMiembroRepository 
    {
        public GrupoMiembroRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    /// <summary>
    /// Titulares de Patentes
    /// </summary>
    public class PatTitularRepository : Repository<TitularEnPatentes>, IPatTitularRepository 
    {

        private const string SQL_BUSQUEDA_PATTITULAR = @"
        SELECT TOP 50 T.*
        FROM patentes.PTitulares T
        INNER JOIN CONTAINSTABLE (patentes.PTitulares, Nombre, '""*[t-s]*""') AS KEY_TBL ON  T.Id = KEY_TBL.[KEY]
        ORDER BY KEY_TBL.RANK, T.Nombre asc
        ";

        public PagedList searchTitular(string textToSearch)
        {

            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {

                    var sql = SQL_BUSQUEDA_PATTITULAR
                        .Replace("[t-s]", (!string.IsNullOrEmpty(textToSearch) ? textToSearch : ""));

                    var dataset = conn.Query<TitularEnPatentes>(sql);
                    int totalnRecords = -1;

                    result.DataSet = dataset;
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                System.Diagnostics.Trace.TraceError(exception.Message);
            }
            return result;
        }

        public PatTitularRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    /// <summary>
    /// Agentes de Patentes
    /// </summary>
    public class AgenteRepository : Repository<Agente>, IAgenteRepository
    {

        private const string SQL_BUSQUEDA_AGENTE = @"
        SELECT TOP 50 T.Id, T.Nombre
        FROM patentes.agentes T
        INNER JOIN CONTAINSTABLE (patentes.Agentes, Nombre, '""*[t-s]*""') AS KEY_TBL ON  T.Id = KEY_TBL.[KEY]
        ORDER BY KEY_TBL.RANK, T.Nombre asc
        ";

        public PagedList searchAgente(string textToSearch)
        {

            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {

                    var sql = SQL_BUSQUEDA_AGENTE
                        .Replace("[t-s]", (!string.IsNullOrEmpty(textToSearch) ? textToSearch : ""));

                    var dataset = conn.Query<TitularEnPatentes>(sql);
                    int totalnRecords = -1;

                    result.DataSet = dataset;
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                System.Diagnostics.Trace.TraceError(exception.Message);
            }
            return result;
        }
        public AgenteRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
    }

    /// <summary>
    /// Inventores
    /// </summary>
    public class InventorRepository : Repository<Inventor>, IInventorRepository
    {
        public InventorRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        public ResultInfo SaveInventor(GenericEntity<Inventor> model, Auditoria auditoria)
        {
            var result = new ResultInfo() { Succeeded = false };

            var inventor = model.Generic as Inventor; //inventor data

            if (inventor.ExpedienteId == 0)
                return result;

            if (inventor.Id == 0)
            {
                DbContext.Inventores.Add(inventor);
                DbContext.SaveChanges();
            }
            auditoria.Evento += "-" + inventor.Id.ToString();
            SaveAuditoria(auditoria);

            result.Result = inventor;
            result.Succeeded = true;
            return result;
        }

        public ResultInfo DeleteInventor(int inventorId, Auditoria auditoria)
        {
            var inventor = DbContext.Inventores.Where(pt => pt.Id == inventorId).First();
            DbContext.Inventores.Remove(inventor);
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


//DECLARE @TokensTbl table(TokenId int, Position int);
//DECLARE @WordsTbl table(WordId int);

//DECLARE @wordTokenId INT;
//DECLARE @tokenId INT;
//DECLARE @word NVARCHAR(100);

//    DECLARE words_cursor CURSOR FOR 
//    SELECT word
//    FROM SplitWords('GUATEAMALA')

//    OPEN words_cursor
//    FETCH NEXT FROM words_cursor INTO @word;
//    WHILE @@FETCH_STATUS = 0
//    BEGIN
		
//        DECLARE @lenght INT = LEN(@word);
//        DECLARE @ctrl INT=1;
//        DECLARE @position INT=1;
//        DECLARE @token nvarchar(3)='';
//        WHILE @ctrl<=@lenght
//        BEGIN
//            IF (@lenght<=3)
//                BEGIN
//                    SET @token = @word;
//                    SET @ctrl = @lenght;
//                END
//            ELSE
//                BEGIN
//                    SET @token = SUBSTRING(@word, @ctrl, 3);
//                    IF (@ctrl+2=@lenght)
//                        SET @ctrl = @lenght;
//                END

//            SET @tokenId = (SELECT Id FROM [ip_search].[Tokens] WHERE Token = @token);
//            IF (@tokenId IS  NOT NULL)
//            BEGIN
//                --IF (NOT EXISTS(SELECT * FROM @TokensTbl WHERE TokenId=@tokenId))
//                INSERT INTO @TokensTbl(TokenId, Position) VALUES(@tokenId, @position);
//                SET @position = @position + 1;
//            END

//            PRINT @token
//            SET @ctrl = @ctrl + 1;
//        END

//        SET @wordTokenId = (SELECT Id FROM [ip_search].[WordTokens] WHERE WordToken = @word);
//        IF (@wordTokenId IS  NULL)
//        BEGIN
//            IF (NOT EXISTS(SELECT * FROM @WordsTbl WHERE WordId=@wordTokenId))
//                INSERT INTO @WordsTbl(WordId) VALUES(@wordTokenId);
//        END

//        PRINT @word
//        FETCH NEXT FROM words_cursor INTO @word;
//    END

//    CLOSE words_cursor;
//    DEALLOCATE words_cursor;

//WITH ExpTokensCTE AS 
//(
//    SELECT ET.ExpedienteId, 10 AS Ranking
//    FROM @TokensTbl T
//    INNER JOIN [ip_search].[ExpTokens] ET ON T.TokenId = ET.TokenId AND ET.Position = T.Position

//    UNION ALL

//    SELECT ET.ExpedienteId, (CASE WHEN ABS(T.Position - ET.Position)<3 THEN 5 ELSE 0 END) AS Ranking
//    FROM @TokensTbl T
//    INNER JOIN [ip_search].[ExpTokens] ET ON T.TokenId = ET.TokenId 
//)
//SELECT TOP 50 M.ExpedienteId, M.Denominacion, SUM(Ranking) AS ZRanking
//FROM dbo.Marcas M
//INNER JOIN ExpTokensCTE T ON M.ExpedienteId = T.ExpedienteId
//GROUP BY M.ExpedienteId, M.Denominacion
//ORDER BY ZRanking desc


