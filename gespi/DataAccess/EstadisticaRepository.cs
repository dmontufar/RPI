using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PI.DataAccess
{

    public interface IEstadisticaRepository : IRepository<ExpedientesByArea>
    {
        PagedList GetEstadisticasByArea(int year);
        PagedList GetIngresoExpedientesPorMes(int year);
    }


    public class EstadisticaRepository : Repository<ExpedientesByArea>, IEstadisticaRepository
    {
        public EstadisticaRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }


        private const string GET_ESTADISTICA_BY_AREA = @"
SELECT (
	SELECT Count(*) 
	from dbo.Expedientes e
	WHERE e.ModuloId=3  AND e.EstatusId=130 AND YEAR(e.FechaDeSolicitud)=[y]
) AS DAIngreso,
(
	SELECT Count(*)
	from dbo.Expedientes e
	WHERE e.ModuloId=1  AND e.EstatusId=7 AND YEAR(e.FechaDeSolicitud)=[y]
) AS SignosIngreso,
(
	SELECT Count(*) 
	from dbo.Expedientes e
	WHERE e.ModuloId=2  AND e.EstatusId=59 AND YEAR(e.FechaDeSolicitud)=[y]
) AS PatentesIngreso,
(
	SELECT Count(*) 
	from dbo.Expedientes e
	WHERE e.ModuloId=3  AND e.EstatusId=140 AND YEAR(e.FechaDeSolicitud)=[y] -- pendiente de registrar
) AS DAPublicacion,
(
	SELECT Count(*)
	from dbo.Expedientes e
	WHERE e.ModuloId=1  AND e.EstatusId=67 AND YEAR(e.FechaDeSolicitud)=[y]
) AS SignosPublicacion,
(
	SELECT Count(*) 
	from dbo.Expedientes e
	WHERE e.ModuloId=2  AND e.EstatusId=59 AND YEAR(e.FechaDeSolicitud)=[y]
) AS PatentesPublicacion,
(
	SELECT Count(*) 
	from dbo.Expedientes e
	WHERE e.ModuloId=3  AND e.EstatusId=135 AND YEAR(e.FechaDeSolicitud)=[y] -- con titulo
) AS DAConTitulo,
(
	SELECT Count(*)
	from dbo.Expedientes e
	WHERE e.ModuloId=1  AND e.EstatusId=41 AND YEAR(e.FechaDeSolicitud)=[y]
) AS SignosConTitulo,
(
	SELECT Count(*) 
	from dbo.Expedientes e
	WHERE e.ModuloId=2  AND e.EstatusId=88 AND YEAR(e.FechaDeSolicitud)=[y]
) AS PatentesConTitulo
";

        public PagedList GetEstadisticasByArea(int year)
        {
            if (year == 0)
                year = DateTime.Now.Year;

            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var favoritos = conn.Query<ExpedientesByArea>(
                        GET_ESTADISTICA_BY_AREA.Replace("[y]", year.ToString())
                        );

                    int totalnRecords = 0;
                    result.DataSet = favoritos;
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

        private const string GET_INGRESO_EXPEDIENTES_POR_MES = @"
SELECT e.ModuloId, MONTH(e.FechaDeSolicitud) AS MES, Count(*) As Conteo
	from dbo.Expedientes e
	WHERE e.ModuloId=3  AND e.EstatusId=130 --AND YEAR(e.FechaDeSolicitud)=[y]
	GROUP BY e.ModuloId, MONTH(e.FechaDeSolicitud)
UNION
SELECT e.ModuloId, MONTH(e.FechaDeSolicitud) AS MES, Count(*) As Conteo
	from dbo.Expedientes e
	WHERE e.ModuloId=1  AND e.EstatusId=7 --AND YEAR(e.FechaDeSolicitud)=[y]
	GROUP BY e.ModuloId, MONTH(e.FechaDeSolicitud)
UNION
SELECT e.ModuloId, MONTH(e.FechaDeSolicitud) AS MES, Count(*) As Conteo
	from dbo.Expedientes e
	WHERE e.ModuloId=2  AND e.EstatusId=59 --AND YEAR(e.FechaDeSolicitud)=[y]
	GROUP BY e.ModuloId, MONTH(e.FechaDeSolicitud)
        ";

        public PagedList GetIngresoExpedientesPorMes(int year)
        {
            if (year == 0)
                year = DateTime.Now.Year;

            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var favoritos = conn.Query<IngresoExpedientesMensual>(
                        GET_INGRESO_EXPEDIENTES_POR_MES.Replace("[y]", year.ToString())
                        );

                    int totalnRecords = 0;
                    result.DataSet = favoritos;
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
    }
}
