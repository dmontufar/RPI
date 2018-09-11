using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;

namespace DataImport
{
    public class Patentes
    {
        //public const string ConnectionString = "Data Source=.;Initial Catalog=GPI;Integrated Security=True";
        public static string ConnectionString;

        //static string dbaseConnString = "Provider=vfpoledb;Data Source=" + @"C:\MyProjects\oldRPIdb\DBPAT" + ";Extended Properties=dBASE IV;";
        static string dbaseConnString = "Provider=vfpoledb;Data Source={0};Extended Properties=dBASE IV;";

        public static SqlConnection connection;

        public static SqlConnection GetOpenConnection()
        {
            ConnectionString = ConfigurationManager.AppSettings["rpiDb"];
            if (connection == null)
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();
            }
            return connection;
        }

        public static OleDbConnection GetdbaseOpenConnection()
        {
            string FilePath = ConfigurationManager.AppSettings["legacyPatentes"];

            var conn = new OleDbConnection(string.Format(dbaseConnString, FilePath));
            conn.Open();
            return conn;

        }

        private static SqlConnection cnn = GetOpenConnection();
        private static OleDbConnection dbaseConn = GetdbaseOpenConnection();

        static Patentes() {
            cnn = GetOpenConnection();
            dbaseConn = GetdbaseOpenConnection();
        }

        public static void ImportEstatus()
        {
            string DBF_FileName = "status";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {              
                string idstatus = dataReader.GetValue(0).ToString();
                string descripcion = dataReader.GetString(2);

                int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Estatus([Codigo],[Descripcion],[ModuloId])
                    VALUES( @Codigo, @Descripcion, @ModuloId);
                    SELECT SCOPE_IDENTITY() AS [EstatusId]; ",
                    new 
                    { 
                        Codigo=idstatus, 
                        Descripcion=descripcion,  
                        ModuloId = 2
                    }
                    ).Single();
            }
            dataReader.Close();
        }

        public static void ImportAgentes()
        {
            string DBF_FileName = "agentes";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {

                string idagente = dataReader.GetValue(0).ToString();
                string agente = dataReader.GetString(1);
                try
                {
                    string domicilio = dataReader.GetString(2);
                    string telefono = dataReader.GetString(3);
                    string fax = dataReader.GetString(4);
                    int expedienteId = cnn.Query<int>(@"INSERT INTO patentes.Agentes([Nombre],[Domicilio],[Telefono],[Fax],[tmpId])
                    VALUES( @Nombre, @Domicilio, @Telefono, @Fax, @tmpId);
                    SELECT SCOPE_IDENTITY() AS [AgenteId]; ",
                        new
                        {
                            Nombre = agente,
                            Domicilio = domicilio,
                            Telefono = telefono,
                            Fax = fax,
                            tmpId = idagente,
                        }
                        ).Single();
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Agente {0}, {1} tiene errores", idagente, agente));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        }

        public static void ImportClasificaciones()
        {
            if (true)
                return; //

            string DBF_FileName = "clasificacion";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {              
                string idclasifica = dataReader.GetValue(0).ToString();
                string descripcion = dataReader.GetString(1);

                int expedienteId = cnn.Query<int>(@"INSERT INTO patentes.Clasificaciones([Descripcion],[tmpId])
                    VALUES( @Descripcion, @tmpId);
                    SELECT SCOPE_IDENTITY() AS [EstatusId]; ",
                    new 
                    { 
                        Descripcion=descripcion, 
                        tmpId=idclasifica
                    }
                    ).Single();
            }
            dataReader.Close();
        }

        public static void ImportExpedientes()
        {
            string DBF_FileName = "mainpat";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {              
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    DateTime? fecha_solicitud = dataReader.GetDateTime(2); //date
                    string horaingreso = dataReader.GetString(3);
                    string registro = dataReader.GetValue(4).ToString();
                    string idstatus = dataReader.GetString(5);
                    DateTime? fecha_status = dataReader.GetDateTime(6);

                    string idagente = dataReader.GetValue(11).ToString();
                    string nanualidades = dataReader.GetValue(12).ToString();
                    string folio = dataReader.GetValue(13).ToString();
                    string tomo = dataReader.GetValue(14).ToString();
                    string descripcion = dataReader.GetString(15);
                    string ley = dataReader.GetString(16);

                    string resumen = dataReader.GetString(18);
                    string iduser_rec = dataReader.GetString(23);
                    DateTime? fecha_rec = dataReader.GetDateTime(24);
                    string idclasifica = dataReader.GetString(27);

                    string pct_id = dataReader.GetString(32);
                    DateTime? pct_fecha = dataReader.GetDateTime(33);
                    string citaciones = dataReader.GetString(34);

                    //DateTime FechaDeSolicitud = (string.IsNullOrEmpty(fecha_solicitud.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(fecha_solicitud.Substring(0, 4)), int.Parse(fecha_solicitud.Substring(4, 2)), int.Parse(fecha_solicitud.Substring(6, 2))));
                    //DateTime FechaDeEstatus = (string.IsNullOrEmpty(fecha_status.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(fecha_status.Substring(0, 4)), int.Parse(fecha_status.Substring(4, 2)), int.Parse(fecha_status.Substring(6, 2))));

                    var timex = horaingreso.Split(':').ToList<string>();
                    if (timex.Count() == 2) timex.Add("");

                    TimeSpan time = (timex.Count() < 2) ? new TimeSpan(0, 0, 0) : new TimeSpan(parseToInt(timex[0]), parseToInt(timex[1]), parseToInt(timex[2]));

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = idstatus }).FirstOrDefault();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int agenteId = cnn.Query<int>("select Id FROM patentes.Agentes WHERE tmpId like @tmpId", new { tmpId = idagente }).FirstOrDefault();

                    int clasificacionId = cnn.Query<int>("select Id FROM patentes.Clasificaciones WHERE tmpId like @tmpId", new { tmpId = idclasifica }).FirstOrDefault();

                    int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId])
                    VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId);
                    SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                        new
                        {
                            ModuloId = 2,
                            TipoDeRegistroId = tipoDeRegistroId,
                            Numero = idsolicitud,
                            FechaDeSolicitud = fecha_solicitud,
                            Hora = time,
                            EstatusId = statusId,
                            FechaDeEstatus = fecha_status,
                            LeyId = ley
                        }
                        ).Single();

                    cnn.Execute(@"INSERT INTO patentes.Patentes(
                        [ExpedienteId],
                        [Descripcion],
                        [Registro],
                        [AgenteId],
                        [anualidades],
                        [Folio],
                        [Tomo],
                        [Resumen],
                        [ClasificacionId],
                        [RecibidoPorUsuarioId],
                        [FechaRecepcion],
                        [Pct],
                        [Fecha_Pct],
                        [Citaciones])
                        VALUES(
                        @ExpedienteId,
                        @Descripcion,
                        @Registro,
                        @AgenteId,
                        @anualidades,
                        @Folio,
                        @Tomo,
                        @Resumen,
                        @ClasificacionId,
                        @RecibidoPorUsuarioId,
                        @FechaRecepcion,
                        @Pct,
                        @Fecha_Pct,
                        @Citaciones
                        )",
                         new
                         {
                             ExpedienteId = expedienteId,
                             Descripcion = descripcion,
                             Registro = registro,
                             AgenteId = agenteId,
                             anualidades = nanualidades,
                             Folio = folio,
                             Tomo = tomo,
                             Resumen = resumen,
                             ClasificacionId = clasificacionId,
                             RecibidoPorUsuarioId = 0,
                             FechaRecepcion = fecha_rec,
                             Pct = pct_id,
                             Fecha_Pct = pct_fecha,
                             Citaciones = citaciones
                         }
                        );
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        }

        public static int parseToInt(string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return 0;
            else 
                return int.Parse(value);
        }

        public static void ImportCrono()
        {
            string DBF_FileName = "cronologia";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    DateTime fecha = dataReader.GetDateTime(2);
                    string idstatus = dataReader.GetString(4);
                    string referencia = dataReader.GetString(5);
                    string idusuario = dataReader.GetString(6);
                    string observaciones = dataReader.GetString(7);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=2 AND Codigo = @codigo", new { codigo = idstatus }).FirstOrDefault();

                    int usuarioId = 0;
                    if (expedienteId > 0)
                    {
                        Console.WriteLine("expediente " + idsolicitud);

                        cnn.Execute(@"INSERT INTO dbo.Cronologia([ExpedienteId]
                            ,[Fecha]
                            ,[EstatusId]
                            ,[Referencia]
                            ,[UsuarioId]
                            ,[Observaciones]
                            ,[UsuarioIniciales])
                        VALUES(@ExpedienteId,
                            @Fecha,
                            @EstatusId,
                            @Referencia,
                            @UsuarioId,
                            @Observaciones,
                            @UsuarioIniciales
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 Fecha = fecha,
                                 EstatusId = statusId,
                                 Referencia = referencia,
                                 UsuarioId = 0,
                                 Observaciones = observaciones,
                                 UsuarioIniciales = idusuario
                             }
                            );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        }

        /// ABANDONO
        public static void ImportAbandono()
        {
            string DBF_FileName = "abandono";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    string tipo_resolucion = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string choices = dataReader.GetString(4);
                    DateTime fecha1 = dataReader.GetDateTime(5);
                    DateTime fecha2 = dataReader.GetDateTime(6);
                    DateTime fecha3 = dataReader.GetDateTime(7);
                    DateTime fecha4 = dataReader.GetDateTime(8);
                    string observaciones = dataReader.GetString(9);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO patentes.Abandonos([ExpedienteId]
                      ,[TipoResolucion]
                      ,[Fecha]
                      ,[Opciones]
                      ,[Fecha1]
                      ,[Fecha2]
                      ,[Fecha3]
                      ,[Fecha4]
                      ,[Observaciones])
                        VALUES(@ExpedienteId,
                        @TipoResolucion,
                        @Fecha,
                        @Opciones,
                        @Fecha1,
                        @Fecha2,
                        @Fecha3,
                        @Fecha4,
                        @Observaciones)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 TipoResolucion = tipo_resolucion,
                                 Fecha = fecha,
                                 Opciones = choices,
                                 Fecha1 = fecha1,
                                 Fecha2 = fecha2,
                                 Fecha3 = fecha3,
                                 Fecha4 = fecha4,
                                 Observaciones = observaciones
                             }
                            );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("Abandono {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } // END ABANDONO


        /// Anualidades
        public static void ImportAnualidades()
        {
            string DBF_FileName = "anualidades";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetString(0);
                string tipo_registro = dataReader.GetValue(1).ToString();
                try
                {
                    int numeroAnualidad = int.Parse(dataReader.GetValue(2).ToString());
                    DateTime fAnualidad = dataReader.GetDateTime(3);
                    string esrenovacion = dataReader.GetValue(4).ToString();
                    DateTime fVencimiento = dataReader.GetDateTime(5);
                    double valor = double.Parse(dataReader.GetValue(2).ToString());
                    string recibo = dataReader.GetString(7);
                    DateTime fRecibo = dataReader.GetDateTime(8);
                    string usuario = string.Empty; // PENDIENTE!!
                    string observaciones = dataReader.GetString(9);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();


                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO patentes.Anualidades([ExpedienteId]
                          ,[NumeroAnualidad]
                          ,[EsRenovacion]
                          ,[FechaVencimiento]
                          ,[Valor]
                          ,[Recibo]
                          ,[FechaRecibo]
                          ,[UsuarioId]
                          ,[Observaciones]
                          ,[FechaAnualidad])
                        VALUES(@ExpedienteId,
                          @NumeroAnualidad,
                          @EsRenovacion,
                          @FechaVencimiento,
                          @Valor,
                          @Recibo,
                          @FechaRecibo,
                          @UsuarioId,
                          @Observaciones,
                          @FechaAnualidad)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 NumeroAnualidad = numeroAnualidad,
                                 EsRenovacion = esrenovacion == "S",
                                 FechaVencimiento = fVencimiento,
                                 Valor = valor,
                                 Recibo = recibo,
                                 FechaRecibo = fVencimiento,
                                 UsuarioId = 0,
                                 Observaciones = observaciones,
                                 FechaAnualidad = fAnualidad
                             }
                            );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Anualidad {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Anualidades


        /// DoctosExt
        public static void ImportDoctosExt()
        {
            string DBF_FileName = "doctosext";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    string file = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string descripcion = dataReader.GetValue(4).ToString();
                    string usuario = dataReader.GetValue(5).ToString(); // PENDIENTE!!

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();
                    int cronologiaId = cnn.Query<int>("SELECT TOP 1 Id FROM [dbo].[Cronologia] WHERE ExpedienteId = @expedienteId AND Fecha=@fecha", new { expedienteId = expedienteId, fecha = fecha }).FirstOrDefault();


                    if (expedienteId > 0 && cronologiaId > 0) //cronologiaId=0 resolucion sin cronologia
                    {
                        log.Error(string.Format("found expediente id {0}", expedienteId));
                        log.Error(cronologiaId.ToString());
                        // cambiar nombre
                        cnn.Execute(@"INSERT INTO dbo.ExtResoluciones([CronologiaId]
                          ,[Resolucion]
                          ,[Descripcion])
                        VALUES(@CronologiaId,
                          @Resolucion,
                          @Descripcion)",
                            new
                            {
                                CronologiaId = cronologiaId,
                                Resolucion = file,
                                Descripcion = descripcion
                            }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Docto ext {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END DoctosExt


        /// Inventores
        public static void ImportInventores()
        {
            string DBF_FileName = "inventores";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    string nombre = dataReader.GetString(2);
                    string direccion = dataReader.GetString(3);
                    string ciudad = dataReader.GetString(4);
                    string pais = dataReader.GetString(5);
                    string telefono = dataReader.GetString(6);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();


                    if (expedienteId > 0 && paisId > 0)
                    {
                        log.Error(string.Format("found expediente id {0}", expedienteId));
                        log.Error(paisId.ToString());
                        cnn.Execute(@"INSERT INTO patentes.Inventores([ExpedienteId]
                                    ,[Nombre]
                                    ,[Direccion]
                                    ,[Ciudad]
                                    ,[PaisId]
                                    ,[Telefono]
                                    )
                        VALUES(@ExpedienteId,
                          @Nombre,
                          @Direccion,
                          @Ciudad,
                          @PaisId,
                          @Telefono)",
                        new
                        {
                            ExpedienteId = expedienteId,
                            Nombre = nombre,
                            Direccion = direccion,
                            Ciudad = ciudad,
                            PaisId = paisId,
                            Telefono = telefono
                        }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Inventor {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Inventores

        /// IPC
        /// add another column char of two and move indice to that column then remove all dirty data
        public static void ImportIPC() //Create index column from indice char of 2, -- REMOVE SET FILTER TO index="**"
        {
            string DBF_FileName = "ipc";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    int indice = int.Parse(dataReader.GetValue(6).ToString());
                    string clas_int = dataReader.GetString(3);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();


                    if (expedienteId > 0 && !string.IsNullOrEmpty(clas_int.Trim()))
                    {
                        log.Error(string.Format("found expediente id {0}", expedienteId));
                        cnn.Execute(@"INSERT INTO patentes.IPC([ExpedienteId]
                                    ,[Indice]
                                    ,[Classificacion]
                                    )
                        VALUES(@ExpedienteId,
                          @Indice,
                          @Classificacion)",
                        new
                        {
                            ExpedienteId = expedienteId,
                            Indice = indice,
                            Classificacion = clas_int
                        }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("IPC {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END IPC


        /// Prioridades
        public static void ImportPrioridades()
        {
            string DBF_FileName = "PRIORIDAD";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                try
                {
                    string pais = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string tipo_referencia = dataReader.GetString(4);
                    string solicitudp = dataReader.GetString(5);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();


                    if (expedienteId > 0 && paisId > 0)
                    {
                        log.Error(string.Format("found expediente id {0}", expedienteId));
                        cnn.Execute(@"INSERT INTO patentes.Prioridades([ExpedienteId]
                                    ,[PaisId]
                                    ,[Fecha]
                                    ,[Tipo_referencia]
                                    ,[SolicitudP]
                                    )
                        VALUES(@ExpedienteId,
                          @PaisId,
                          @Fecha,
                          @Tipo_referencia,
                          @SolicitudP)",
                        new
                        {
                            ExpedienteId = expedienteId,
                            PaisId = paisId,
                            Fecha = fecha,
                            Tipo_referencia = tipo_referencia,
                            SolicitudP = solicitudp
                        }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Prioridad {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Prioridades


        /// Titulares
        public static void ImportTitulares()
        {
            string DBF_FileName = "titular";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idtitular = dataReader.GetValue(0).ToString();
                string nombre = dataReader.GetString(1);
                string pais = dataReader.GetString(2);

                try
                {
                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();

                    if (paisId > 0)
                    {
                        log.Error(string.Format("titular {0} pais {1}", idtitular, pais));
                        cnn.Execute(@"INSERT INTO patentes.TitularesEnPatentes([Nombre]
                                    ,[PaisId]
                                    ,[tmpId]
                                    )
                        VALUES(@Nombre,
                          @PaisId,
                          @tmpId)",
                        new
                        {
                            Nombre = nombre,
                            PaisId = paisId,
                            tmpId = idtitular
                        }
                        );
                    }
                    else
                    {
                        log.Error(idtitular);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("TitularesEnPatentes {0}, {1} tiene errores", idtitular, nombre));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Titulares


        /// TitularesDeLaPatente
        public static void ImportTitularesDeLaPatente()
        {
            string DBF_FileName = "titulares_x_pat";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                string idtitular = dataReader.GetValue(2).ToString();
                try
                {
                    string direcciontit = dataReader.GetString(3);
                    string pais = dataReader.GetString(4);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();
                    int titularId = cnn.Query<int>("SELECT Id FROM patentes.TitularesEnPatentes WHERE tmpId = @tmpId", new { tmpId = idtitular }).FirstOrDefault();

                    if (expedienteId > 0 && titularId > 0)
                    {
                        log.Error(string.Format("found expediente id {0} titular {1}", expedienteId, titularId));
                        cnn.Execute(@"INSERT INTO patentes.TitularesDeLaPatente([ExpedienteId]
                                    ,[TitularId]
                                    ,[Direccion]
                                    ,[PaisId]
                                    )
                        VALUES(@ExpedienteId,
                          @TitularId,
                          @Direccion,
                          @PaisId)",
                        new
                        {
                            ExpedienteId = expedienteId,
                            TitularId = titularId,
                            Direccion = direcciontit,
                            PaisId = paisId
                        }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Titular de la patente {0}, {1}, {2} tiene errores", tipo_registro, idsolicitud, idtitular));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END TitularesDeLaPatente


        /// Titulos
        public static void ImportTitulos()
        {
            string DBF_FileName = "TITULOS";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                string tipo_resolucion = dataReader.GetString(2);
                try
                {
                    DateTime fecha = dataReader.GetDateTime(3);
                    string observaciones = dataReader.GetString(4);
                    DateTime fecha_anotacion = dataReader.GetDateTime(5);
                    DateTime fecha_consecion = dataReader.GetDateTime(6);
                    string plazo = dataReader.GetValue(7).ToString();
                    string clasificacion = dataReader.GetValue(8).ToString();
                    string flag = dataReader.GetValue(9).ToString();
                    string tipo_acuerdo = dataReader.GetValue(11).ToString();
                    string acuerdo = dataReader.GetValue(12).ToString();
                    DateTime fecha_acuerdo = dataReader.GetDateTime(13);



                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error(string.Format("found expediente id {0} titular {1}", expedienteId, tipo_resolucion));
                        cnn.Execute(@"INSERT INTO patentes.Titulos([ExpedienteId]
                                    ,[TipoResolucion]
                                    ,[Fecha]
                                    ,[Observaciones]
                                    ,[FechaAnotaciones]
                                    ,[FechaConsecion]
                                    ,[Plazo]
                                    ,[Clasificacion]
                                    ,[Flag]
                                    ,[TipoDeAcuerdoId]
                                    ,[Acuerdo]
                                    ,[FechaAcuerdo]
                                    )
                        VALUES(@ExpedienteId,
                          @TipoResolucion,
                          @Fecha,
                          @Observaciones,
                          @FechaAnotaciones,
                          @FechaConsecion,
                          @Plazo,
                          @Clasificacion,
                          @Flag,
                          @TipoDeAcuerdoId,
                          @Acuerdo,
                          @FechaAcuerdo
                        )",
                        new
                        {
                            ExpedienteId = expedienteId,
                            TipoResolucion = tipo_resolucion,
                            Fecha = fecha,
                            Observaciones = observaciones,
                            FechaAnotaciones = fecha_anotacion,
                            FechaConsecion = fecha_consecion,
                            Plazo = plazo,
                            Clasificacion = clasificacion,
                            Flag = flag,
                            TipoDeAcuerdoId = tipo_acuerdo,
                            Acuerdo = acuerdo,
                            FechaAcuerdo = fecha_acuerdo
                        }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Titulo de la patente {0}, {1}, {2} tiene errores", tipo_registro, idsolicitud, tipo_resolucion));
                    log.Error(exception.Message);
                }

            }
            dataReader.Close();
        } // END Titulos


        /// [Resoluciones]-Transacciones.dbf
        public static void ImportResoluciones()
        {
            string DBF_FileName = "transacciones";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string tipo_registro = dataReader.GetValue(0).ToString();
                string idsolicitud = dataReader.GetString(1);
                string tipo_resolucion = dataReader.GetString(2);
                try
                {
                    DateTime fecha = dataReader.GetDateTime(3);
                    string observaciones = dataReader.GetString(4);
                    string choices = dataReader.GetString(5);
                    string otros = dataReader.GetString(6);
                    DateTime fechanotificacionobs = dataReader.GetDateTime(7);
                    DateTime fechapublicacion = dataReader.GetDateTime(8);
                    string hora = dataReader.GetValue(9).ToString();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE TipoDeRegistroId = @tipoDeRegistroId AND Numero = @numero", new { tipoDeRegistroId = tipoDeRegistroId, numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error(string.Format("found expediente id {0} TipoResolucion {1}", expedienteId, tipo_resolucion));
                        cnn.Execute(@"INSERT INTO patentes.Resoluciones([ExpedienteId]
                                    ,[TipoResolucion]
                                    ,[Fecha]
                                    ,[Observaciones]
                                    ,[Opciones]
                                    ,[Otros]
                                    ,[FechaNotificacion]
                                    ,[FechaPublicacion]
                                    ,[Hora]
                                    )
                        VALUES(@ExpedienteId,
                            @TipoResolucion,
                            @Fecha,
                            @Observaciones,
                            @Opciones,
                            @Otros,
                            @FechaNotificacion,
                            @FechaPublicacion,
                            @Hora
                        )",
                        new
                        {
                            ExpedienteId = expedienteId,
                            TipoResolucion = tipo_resolucion,
                            Fecha = fecha,
                            Observaciones = observaciones,
                            Opciones = choices,
                            Otros = otros,
                            FechaNotificacion = fechanotificacionobs,
                            FechaPublicacion = fechapublicacion,
                            Hora = hora
                        }
                        );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Resolucion de la patente {0}, {1}, {2} tiene errores", tipo_registro, idsolicitud, tipo_resolucion));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END [Resoluciones]-Transacciones

    }
}
