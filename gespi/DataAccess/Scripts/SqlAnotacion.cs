using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Scripts
{
    public class SqlAnotacion
    {
        /*
         * Expedientes en anotacion
         */
        public static string SQL_EXPEDIENTES_DE_MARCA_EN_ANOTACION = @"
        SELECT a.Id,
              a.ExpedienteId,
              a.EnRegistro,
              a.Raya,
              a.EnExpedienteId,
              a.EstatusId,
              a.ActualizadoPorUsuarioId,
              a.FechaActualizacion,	  
	          --m.Registro AS AnotacionEnRegistro, 
	          em.numero AS NumeroDeExpedienteMarca, 
	          --m.Raya, 
	          tr.codigo AS TipoDeRegistroDeLaMarca, 
	          m.Denominacion, 
	          t.Nombre AS NombreTitular, 
	          crono.Fecha FechaDeRegistro, 
	          em.FechaDeEstatus FechaDeRegistroEnMarcas,
	          niza.Codigo AS ClasificacionNiza
        FROM anotacionEnExpedientes a
        INNER JOIN expedientes e ON (a.ExpedienteId = e.Id)
        INNER JOIN marcas m ON (a.EnRegistro = m.Registro)
        INNER JOIN expedientes em ON (m.ExpedienteId = em.id)
        INNER JOIN TiposDeRegistro tr ON (em.TipoDeRegistroId = tr.Id)
        LEFT JOIN ClassificacionDeNiza niza ON (m.ClassificacionDeNizaId = niza.Id)
        LEFT JOIN Cronologia crono ON (m.ExpedienteId = crono.ExpedienteId AND crono.EstatusId = 41)
        LEFT JOIN TitularesDeLaMarca titm ON (m.ExpedienteId = titm.ExpedienteId)
        LEFT JOIN Titulares t ON (titm.TitularId = t.Id)
        WHERE e.Id =  [ExpedienteId]
        ORDER BY m.Denominacion
        ";
        /* END >> SQL_EXPEDIENTES_DE_MARCA_EN_ANOTACION*/

    }
}
