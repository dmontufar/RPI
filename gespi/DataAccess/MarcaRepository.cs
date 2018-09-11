using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PI.DataAccess
{
    public class MarcaRepository : Repository<Marca>, IMarcaRepository
    {
        public MarcaRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }

        private const string SQL_BUSQUEDA_TITULAR = @"
        SELECT TOP 50 T.*
        FROM dbo.Titulares T
        INNER JOIN CONTAINSTABLE (Titulares, Nombre, '""*[t-s]*""') AS KEY_TBL ON  T.Id = KEY_TBL.[KEY]
        ORDER BY KEY_TBL.RANK, T.Nombre asc
        ";

        public PagedList searchTitular(string textToSearch)
        {

            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {

                    var sql = SQL_BUSQUEDA_TITULAR
                        .Replace("[t-s]", (!string.IsNullOrEmpty(textToSearch) ? textToSearch : ""));

                    var dataset = conn.Query<Titular>(sql);
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
       
    }
}
