using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Scripts
{
    public class SqlMarca
    {
        /*
         * BUSQUEDA FONETICA
         */
        public static string SQL_BUSQUEDA_FONETICA = @"
        DECLARE @TokensTbl table(TokenId int, Position int);
        DECLARE @WordsTbl table(WordId int);

        DECLARE @wordTokenId INT;
        DECLARE @tokenId INT;
        DECLARE @word NVARCHAR(100);

	        DECLARE words_cursor CURSOR FOR 
	        SELECT word
	        FROM SplitWords('[t-s]');

	        OPEN words_cursor
	        FETCH NEXT FROM words_cursor INTO @word;
	        WHILE @@FETCH_STATUS = 0
	        BEGIN
		
		        DECLARE @lenght INT = LEN(@word);
		        DECLARE @ctrl INT=1;
		        DECLARE @position INT=1;
		        DECLARE @token nvarchar(3)='';
		        WHILE @ctrl<=@lenght
		        BEGIN
			        IF (@lenght<=3)
				        BEGIN
					        SET @token = @word;
					        SET @ctrl = @lenght;
				        END
			        ELSE
				        BEGIN
					        SET @token = SUBSTRING(@word, @ctrl, 3);
					        IF (@ctrl+2=@lenght)
						        SET @ctrl = @lenght;
				        END

			        SET @tokenId = (SELECT Id FROM [ip_search].[Tokens] WHERE Token = @token);
			        IF (@tokenId IS  NOT NULL)
			        BEGIN
				        --IF (NOT EXISTS(SELECT * FROM @TokensTbl WHERE TokenId=@tokenId))
				        INSERT INTO @TokensTbl(TokenId, Position) VALUES(@tokenId, @position);
				        SET @position = @position + 1;
			        END

			        PRINT @token
			        SET @ctrl = @ctrl + 1;
		        END

		        SET @wordTokenId = (SELECT Id FROM [ip_search].[WordTokens] WHERE WordToken = @word);
		        IF (@wordTokenId IS  NULL)
		        BEGIN
			        IF (NOT EXISTS(SELECT * FROM @WordsTbl WHERE WordId=@wordTokenId))
				        INSERT INTO @WordsTbl(WordId) VALUES(@wordTokenId);
		        END

		        PRINT @word
		        FETCH NEXT FROM words_cursor INTO @word;
	        END

	        CLOSE words_cursor;
	        DEALLOCATE words_cursor;

        WITH ExpTokensCTE AS 
        (
	        SELECT ET.ExpedienteId, 10 AS Ranking
	        FROM @TokensTbl T
	        INNER JOIN [ip_search].[ExpTokens] ET ON T.TokenId = ET.TokenId AND ET.Position = T.Position

	        UNION ALL

	        SELECT ET.ExpedienteId, (CASE WHEN ABS(T.Position - ET.Position)<3 THEN 5 ELSE 0 END) AS Ranking
	        FROM @TokensTbl T
	        INNER JOIN [ip_search].[ExpTokens] ET ON T.TokenId = ET.TokenId 
        ),
        QRanking AS 
        (
	        SELECT TOP 50 ExpedienteId, SUM(Ranking) AS ZRanking
	        FROM ExpTokensCTE 
	        GROUP BY ExpedienteId
        )
        SELECT TOP 50 
	            M.ExpedienteId, 
		        e.Numero,
		        M.Denominacion  AS SignoDistintivo, 
		        tr.Nombre AS TipoDeRegistro, 
		        niza.Codigo AS ClasificacionNiza,
		        s.Descripcion AS EstatusDsc, 
		        ZRanking AS Punteo,
		        M.Registro
        FROM dbo.Marcas M
        INNER JOIN dbo.Expedientes e ON M.ExpedienteId = e.Id
        INNER JOIN QRanking T ON M.ExpedienteId = T.ExpedienteId
        INNER JOIN dbo.TiposDeRegistro tr ON e.TipoDeRegistroId = tr.Id
        INNER JOIN dbo.Estatus s ON e.EstatusId = s.Id
        INNER JOIN dbo.ClassificacionDeNiza niza ON M.ClassificacionDeNizaId = niza.Id
        --WHERE M.ClassificacionDeNizaId = 01
        WHERE ZRanking>0 [clases]
        ORDER BY ZRanking desc, M.Denominacion, M.ClassificacionDeNizaId asc        
        ";
        /* END >> BUSQUEDA FONETICA*/



        /*
         * BUSQUEDA IDENTICA
         */
        public static string SQL_BUSQUEDA_IDENTICA = @"
        SELECT TOP 100 
	            M.ExpedienteId, 
		        e.Numero,
		        M.Denominacion  AS SignoDistintivo, 
		        tr.Nombre AS TipoDeRegistro, 
		        niza.Codigo AS ClasificacionNiza,
		        s.Descripcion AS EstatusDsc, 
		        M.Registro, 
		        KEY_TBL.RANK AS Punteo
        FROM dbo.Marcas M
        INNER JOIN CONTAINSTABLE (dbo.Marcas, Denominacion, '""*[t-s]*""') AS KEY_TBL ON  m.Id = KEY_TBL.[KEY]
        INNER JOIN dbo.Expedientes e ON M.ExpedienteId = e.Id
        INNER JOIN dbo.TiposDeRegistro tr ON e.TipoDeRegistroId = tr.Id
        INNER JOIN dbo.Estatus s ON e.EstatusId = s.Id
        INNER JOIN dbo.ClassificacionDeNiza niza ON M.ClassificacionDeNizaId = niza.Id
        [where]
        ORDER BY KEY_TBL.RANK, M.Denominacion, M.ClassificacionDeNizaId asc";

        /* END >> BUSQUEDA IDENTICA */

    }
}
