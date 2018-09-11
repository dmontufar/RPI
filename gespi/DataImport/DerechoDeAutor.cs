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
    public class DerechoDeAutor
    {
        //public const string ConnectionString = "Data Source=.;Initial Catalog=GPI;Integrated Security=True";
        public static string ConnectionString;

        //static string dbaseConnString = "Provider=vfpoledb;Data Source=" + @"C:\MyProjects\oldRPIdb\DAUTORDB" + ";Extended Properties=dBASE IV;";
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
            string FilePath = ConfigurationManager.AppSettings["legacyDA"];

            var conn = new OleDbConnection(string.Format(dbaseConnString, FilePath));            
            conn.Open();
            return conn;

        }

        private static SqlConnection cnn = GetOpenConnection();
        private static OleDbConnection dbaseConn = GetdbaseOpenConnection();

        static DerechoDeAutor()
        {
            cnn = GetOpenConnection();
            dbaseConn = GetdbaseOpenConnection();
        }

        public static void ImportEstatus()
        {
            if (1==1)
                return;

            string DBF_FileName = "status";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {              
                string idstatus = dataReader.GetValue(0).ToString();
                string descripcion = dataReader.GetString(1);

                int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Estatus([Codigo],[Descripcion],[ModuloId])
                    VALUES( @Codigo, @Descripcion, @ModuloId);
                    SELECT SCOPE_IDENTITY() AS [EstatusId]; ",
                    new 
                    { 
                        Codigo=idstatus, 
                        Descripcion=descripcion,  
                        ModuloId = 3
                    }
                    ).Single();
            }
            dataReader.Close();
        }

        public static void ImportFormularios()
        {
            string DBF_FileName = "formularios";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {              
                string codigo = dataReader.GetValue(0).ToString();
                string considerando = dataReader.GetString(2);
                int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=3 AND codigo like @codigo", new { codigo = codigo + "%" }).FirstOrDefault();

                if (tipoDeRegistroId > 0)
                {
                    cnn.Query<int>(@"INSERT INTO da.Formularios([TipoDeRegistroId],[Considerando])
                    VALUES( @TipoDeRegistroId, @Considerando);",
                        new
                        {
                            TipoDeRegistroId = tipoDeRegistroId,
                            Considerando = considerando
                        }
                        );
                }
                else
                {
                    log.Error(string.Format("Formulario no encontrado {0}", codigo));
                }
            }
            dataReader.Close();
        }

        // Expedientes
        public static void ImportExpedientes()
        {
            string DBF_FileName = "datosobra";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);
            log.Error("agregar FECHAP2 a datosobra con tipo fecha y llenarlo con el campo fecha REPLACE fechap2 WITH CTOD(fechap) ALL");
            

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string idstatus = dataReader.GetString(1);
                string idtipo = dataReader.GetString(2);

                try
                {

                    DateTime? fechasolicitud = dataReader.GetDateTime(3);
                    string horaingreso = dataReader.GetString(4);
                    string titulo = dataReader.GetValue(5).ToString();
                    string traduccion = dataReader.GetValue(6).ToString();

                    int paginas = int.Parse(dataReader.GetValue(7).ToString());

                    string formato = dataReader.GetValue(8).ToString();
                    string lugared = dataReader.GetValue(9).ToString();

                    DateTime? fechaed = dataReader.GetDateTime(10);

                    string lugarp = dataReader.GetValue(11).ToString();
                    string fechap = dataReader.GetValue(12).ToString();
                    string editor = dataReader.GetValue(13).ToString();
                    string yearcreacion = dataReader.GetValue(14).ToString();
                    string paisorigen = dataReader.GetValue(15).ToString();
                    bool obra1 = dataReader[16] as int? == 1 ? true : false;
                    bool obra2 = dataReader[17] as int? == 1 ? true : false;
                    bool obra3 = dataReader[18] as int? == 1 ? true : false;
                    bool obra4 = dataReader[19] as int? == 1 ? true : false;
                    bool obra5 = dataReader[20] as int? == 1 ? true : false;
                    bool obra6 = dataReader[21] as int? == 1 ? true : false;
                    bool obra7 = dataReader[22] as int? == 1 ? true : false;

                    string otraclasifica = dataReader.GetValue(23).ToString();
                    string versionesautoriza = dataReader.GetValue(24).ToString();
                    string otrainfoqueidentifique = dataReader.GetValue(25).ToString();
                    string soporte = dataReader.GetValue(26).ToString();
                    string usuario = dataReader.GetValue(27).ToString();
                    int registro = int.Parse(dataReader.GetValue(28).ToString());
                    string libro = dataReader.GetValue(29).ToString();
                    int tomo = int.Parse(dataReader.GetValue(30).ToString());
                    int folio = int.Parse(dataReader.GetValue(31).ToString());
                    
                    string fechap2 = dataReader.GetValue(32).ToString();
                    
                    List<String> timex = new List<String>();
                    
                    log.Error(horaingreso);
                    
                    if (!String.IsNullOrEmpty(horaingreso) && horaingreso.Contains(':'))
                        timex = horaingreso.Split(':').ToList<string>();
                    
                    if (timex.Count() == 2) timex.Add("");
                    TimeSpan time = new TimeSpan(0, 0, 0);
                    
                    try
                    {
                        time = (timex.Count() < 2) ? new TimeSpan(0, 0, 0) : new TimeSpan(parseToInt(timex[0]), parseToInt(timex[1]), parseToInt(timex[2]));
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Expediente {0}, {1}, {2} tiene errores", idsolicitud, idtipo, timex));
                    }
                    

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=3 AND Codigo = @codigo", new { codigo = idstatus }).FirstOrDefault();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=3 AND codigo like @codigo", new { codigo = idtipo + "%" }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = paisorigen }).FirstOrDefault();
                    
                    int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId])
                    VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId);
                    SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                        new
                        {
                            ModuloId = 3, // Derecho de autor 
                            TipoDeRegistroId = tipoDeRegistroId,
                            Numero = idsolicitud,
                            FechaDeSolicitud = fechasolicitud,
                            Hora = time,
                            EstatusId = statusId,
                            FechaDeEstatus = fechasolicitud, // actualizar con dataimport cronologia!!
                            LeyId = "2"
                        }
                        ).Single();

                    cnn.Execute(@"INSERT INTO da.DerechoDeAutor(
                             [ExpedienteId]
                            ,[Titulo]
                            ,[Traduccion]
                            ,[Paginas]
                            ,[Formato]
                            ,[LugarEdicion]
                            ,[FechaEdicion]
                            ,[LugarEraPublicacion]
                            ,[FechaPublicacion]
                            ,[Editor]
                            ,[AnioCreacion]
                            ,[PaisOrigenId]
                            ,[EsInedita]
                            ,[EsPublicada]
                            ,[EsOriginaria]
                            ,[EsDerivada]
                            ,[EsIndividual]
                            ,[EsColectiva]
                            ,[EsEnColaboracion]
                            ,[OtraClasificacionAplicable]
                            ,[VersionesAutorizadas]
                            ,[OtraInfoQueIdentifique]
                            ,[SoporteMaterial]
                            ,[Registro]
                            ,[Libro]
                            ,[Tomo]
                            ,[Folio])
                        VALUES(
                            @ExpedienteId,
                            @Titulo,
                            @Traduccion,
                            @Paginas,
                            @Formato,
                            @LugarEdicion,
                            @FechaEdicion,
                            @LugarEraPublicacion,
                            @FechaPublicacion,
                            @Editor,
                            @AnioCreacion,
                            @PaisOrigenId,
                            @EsInedita,
                            @EsPublicada,
                            @EsOriginaria,
                            @EsDerivada,
                            @EsIndividual,
                            @EsColectiva,
                            @EsEnColaboracion,
                            @OtraClasificacionAplicable,
                            @VersionesAutorizadas,
                            @OtraInfoQueIdentifique,
                            @SoporteMaterial,
                            @Registro,
                            @Libro,
                            @Tomo,
                            @Folio
                        )",
                         new
                         {
                             ExpedienteId = expedienteId,
                             Titulo = titulo,
                             Traduccion = traduccion,
                             Paginas = paginas,
                             Formato = formato,
                             LugarEdicion = lugared,
                             FechaEdicion = fechaed,
                             LugarEraPublicacion = lugarp,
                             FechaPublicacion = fechap2,
                             Editor = editor,
                             AnioCreacion = yearcreacion,
                             PaisOrigenId = paisId,
                             EsInedita = obra1,
                             EsPublicada = obra2,
                             EsOriginaria = obra3,
                             EsDerivada = obra4,
                             EsIndividual = obra5,
                             EsColectiva = obra6,
                             EsEnColaboracion = obra7,
                             OtraClasificacionAplicable = otraclasifica,
                             VersionesAutorizadas = versionesautoriza,
                             OtraInfoQueIdentifique = otrainfoqueidentifique,
                             SoporteMaterial = soporte,
                             Registro = registro,
                             Libro = libro,
                             Tomo = tomo,
                             Folio = folio
                         }
                        );

                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0}, {1} tiene errores", idsolicitud, idtipo));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END Expedientes


        public static int parseToInt(string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return 0;
            else 
                return int.Parse(value);
        }

        /// artliterarias - LiterariasyArtisticas
        public static void ImportLiterariasyArtisticas()
        {
            string DBF_FileName = "artliterarias";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string nombreed = dataReader.GetString(1);

                try
                {
                    string direccioned = dataReader.GetString(2);
                    string nombreimp = dataReader.GetString(3);
                    string direccionimp = dataReader.GetString(4);
                    string nedicion = dataReader.GetString(5);
                    string tamano = dataReader.GetString(6);
                    string clasifica = dataReader.GetString(7);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.LiterariasyArtisticas([ExpedienteId]
                                ,[NombreDelEditor]
                                ,[DireccionEditor]
                                ,[NombreImprenta]
                                ,[DireccionImprenta]
                                ,[Edicion]
                                ,[Tamano]
                                ,[Clasificacion]
                                )
                        VALUES(@ExpedienteId,
                                @NombreDelEditor,
                                @DireccionEditor,
                                @NombreImprenta,
                                @DireccionImprenta,
                                @Edicion,
                                @Tamano,
                                @Clasificacion)",
                             new
                             {
                                ExpedienteId = expedienteId,
                                NombreDelEditor = nombreed,
                                DireccionEditor = direccioned,
                                NombreImprenta = nombreimp,
                                DireccionImprenta = direccionimp,
                                Edicion = nedicion,
                                Tamano = tamano,
                                Clasificacion = clasifica
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
                    log.Error(string.Format("LiterariasyArtisticas {0}, {1} tiene errores", idsolicitud, nombreed));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } // END artliterarias - LiterariasyArtisticas

        /// AudiovisualAutores > audio_autoro
        public static void ImportAudiovisualAutores()
        {
            string DBF_FileName = "audio_autoro";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string autor_obra = dataReader.GetString(1);
                try
                {
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.AudiovisualAutores([ExpedienteId]
                                ,[NombreAutor]
                                )
                        VALUES(@ExpedienteId,
                                @NombreAutor)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 NombreAutor = autor_obra
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
                    log.Error(string.Format("AudiovisualAutores {0}, {1} tiene errores", idsolicitud, autor_obra));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } // END AudiovisualAutores > audio_autoro


        /// AporteAudiovisual ->audioaporte.dbf
        public static void ImportAporteAudiovisual()
        {
            string DBF_FileName = "audioaporte";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string director = dataReader.GetString(1);
                try
                {
                    string genero = dataReader.GetString(2);
                    string clase = dataReader.GetString(3);
                    string metraje = dataReader.GetString(4);
                    string duracion = dataReader.GetString(5);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.AporteAudiovisual([ExpedienteId]
                                  ,[Director]
                                  ,[Genero]
                                  ,[Clase]
                                  ,[Metraje]
                                  ,[Duracion]
                                )
                        VALUES(@ExpedienteId,
                                @Director,
                                @Genero,
                                @Clase,
                                @Metraje,
                                @Duracion
                                )",
                             new
                             {
                                ExpedienteId = expedienteId,
                                Director = director,
                                Genero = genero, 
                                Clase = clase,
                                Metraje = metraje,
                                Duracion = duracion
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
                    log.Error(string.Format("AporteAudiovisual {0}, {1} tiene errores", idsolicitud, director));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END AporteAudiovisual ->audioaporte.dbf

        /// ComposicionAutores > audiocomp.dbf 
        public static void ImportComposicionAutores()
        {
            string DBF_FileName = "audiocomp";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string autor_compo = dataReader.GetString(1);
                try
                {
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.ComposicionAutores([ExpedienteId]
                                ,[NombreAutor]
                                )
                        VALUES(@ExpedienteId,
                                @NombreAutor)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 NombreAutor = autor_compo
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
                    log.Error(string.Format("ComposicionAutores {0}, {1} tiene errores", idsolicitud, autor_compo));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END ComposicionAutores > audiocomp.dbf


        /// GuionAutores - audioguion
        public static void ImportGuionAutores()
        {
            string DBF_FileName = "audioguion";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string autor = dataReader.GetString(1);
                try
                {
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.GuionAutores([ExpedienteId]
                                ,[NombreAutor]
                                )
                        VALUES(@ExpedienteId,
                                @NombreAutor)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 NombreAutor = autor
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
                    log.Error(string.Format("GuionAutores {0}, {1} tiene errores", idsolicitud, autor));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END GuionAutores - audioguion

        /// Autores
        public static void ImportAutores()
        {
            string DBF_FileName = "autores";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string nombre = dataReader.GetString(1);
                try
                {
                    int edad = int.Parse(dataReader.GetValue(2).ToString());

                    string estadocivil = dataReader.GetValue(3).ToString();
                    string profesion = dataReader.GetValue(4).ToString();
                    string domicilio = dataReader.GetValue(5).ToString();
                    string nacionalidad = dataReader.GetValue(6).ToString();
                    string identificacion = dataReader.GetValue(7).ToString();
                    string lugarnoti = dataReader.GetValue(8).ToString();
                    string tel = dataReader.GetValue(9).ToString();
                    string fax = dataReader.GetValue(10).ToString();
                    string email = dataReader.GetValue(11).ToString();

                    DateTime? fechanac = dataReader.GetDateTime(12);
                    DateTime? fechadef = dataReader.GetDateTime(13);
                    string datosbibliograficos = dataReader.GetValue(14).ToString();

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.Autores([ExpedienteId]
                                      ,[NombreAutor]
                                      ,[Edad]
                                      ,[EstadoCivil]
                                      ,[Profesion]
                                      ,[Domicilio]
                                      ,[Nacionalidad]
                                      ,[Identificacion]
                                      ,[LugarNotificacion]
                                      ,[Telefono]
                                      ,[Fax]
                                      ,[Email]
                                      ,[FechaNacimiento]
                                      ,[FechaDef]
                                      ,[DatosBibliograficos]
                                )
                        VALUES(@ExpedienteId,
                                @NombreAutor,
                                @Edad,
                                @EstadoCivil,
                                @Profesion,
                                @Domicilio,
                                @Nacionalidad,
                                @Identificacion,
                                @LugarNotificacion,
                                @Telefono,
                                @Fax,
                                @Email,
                                @FechaNacimiento,
                                @FechaDef,
                                @DatosBibliograficos
                                )",
                             new
                             {
                                ExpedienteId = expedienteId,
                                NombreAutor = nombre,
                                Edad = edad,
                                EstadoCivil = estadocivil,
                                Profesion = profesion,
                                Domicilio = domicilio,
                                Nacionalidad = nacionalidad,
                                Identificacion = identificacion,
                                LugarNotificacion = lugarnoti,
                                Telefono = tel,
                                Fax = fax,
                                Email = email,
                                FechaNacimiento = fechanac,
                                FechaDef = fechadef,
                                DatosBibliograficos = datosbibliograficos 
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
                    log.Error(string.Format("Autores {0}, {1} tiene errores", idsolicitud, nombre));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END Autores


        /// Cronologia
        public static void ImportCrono()
        {
            string DBF_FileName = "cronologia";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetString(0);
                DateTime fecha = dataReader.GetDateTime(1);
                try
                {
                    string idstatus_act = dataReader.GetString(2);
                    string idstatus_ant = dataReader.GetString(3);
                    string referencia = dataReader.GetString(4);
                    string usuario = dataReader.GetString(5);
                    string observaciones = dataReader.GetString(6);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=3 AND Codigo = @codigo", new { codigo = idstatus_act }).FirstOrDefault();

                    int usuarioId = 0;
                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

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
                                 UsuarioIniciales = usuario
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
                    log.Error(string.Format("Cronologia {0}, {1} tiene errores", idsolicitud, fecha));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
            log.Error("DAutor cronologia imported");
        }

        /// FonogramaArtistas - fono_art
        public static void ImportFonogramaArtistas()
        {
            string DBF_FileName = "fono_art";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string autor = dataReader.GetString(1);

                try
                {
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.FonogramaArtistas([ExpedienteId]
                                ,[NombreArtista]
                                )
                        VALUES(@ExpedienteId,
                                @NombreArtista)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 NombreArtista = autor
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
                    log.Error(string.Format("FonogramaArtistas {0}, {1} tiene errores", idsolicitud, autor));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END FonogramaArtistas - fono_art

        /// FonogramaTituloDeObras - fono_aut_ob
        public static void ImportFonogramaTituloDeObras()
        {
            string DBF_FileName = "fono_aut_ob";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string titulo_obra = dataReader.GetString(1);
                string autor = dataReader.GetString(2);

                try
                {
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.FonogramaTituloDeObras([ExpedienteId]
                                ,[TituloObra],[NombreAutor]
                                )
                        VALUES(@ExpedienteId,
                                @TituloObra,
                                @NombreAutor)",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 TituloObra = titulo_obra,
                                 NombreAutor = autor,
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
                    log.Error(string.Format("FonogramaTituloDeObras {0}, {1} tiene errores", idsolicitud, titulo_obra));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END FonogramaTituloDeObras - fono_aut_ob

        ///  Templates - msword
        public static void ImportTemplates()
        {
            string DBF_FileName = "msword";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string estatusId = dataReader.GetValue(0).ToString();
                string descripcion = dataReader.GetString(1);
                string docto = dataReader.GetString(2);

                int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=3 AND Codigo = @codigo", new { codigo = estatusId }).FirstOrDefault();

                try
                {
                    if (statusId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.Templates([EstatusId],[Contenido])
                        VALUES(@EstatusId,@Contenido)",
                             new
                             {
                                 EstatusId = statusId,
                                 Contenido = docto
                             }
                            );
                    }
                    else
                    {
                        log.Error(statusId.ToString());
                    }
                } //end try
                catch (Exception exception)
                {
                    log.Error(exception.Message);
                    log.Error(statusId.ToString());
                }
            }
            dataReader.Close();
        } // END Templates - msword


        //ObrasMusicalesyEscenicas > obrasm
        public static void ImportObrasMusicalesyEscenicas()
        {
            string DBF_FileName = "obrasm";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string partitura = dataReader.GetString(1);

                try
                {
                    string letra = dataReader.GetString(2);
                    string genero = dataReader.GetString(3);
                    bool autor = dataReader[4] as int? == 1 ? true : false;
                    bool musica = dataReader[5] as int? == 1 ? true : false;
                    bool comercial = dataReader[6] as int? == 1 ? true : false;
                    string claseogen = dataReader.GetString(7);
                    string duracion = dataReader.GetString(8);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.ObrasMusicalesyEscenicas([ExpedienteId]
                                    ,[Partitura]
                                    ,[Letra]
                                    ,[Genero]
                                    ,[EsAutor]
                                    ,[EsMusica]
                                    ,[EsComercial]
                                    ,[ClaseGenero]
                                    ,[Duracion]
                                )
                        VALUES(@ExpedienteId,
                                @Partitura,
                                @Letra,
                                @Genero,
                                @EsAutor,
                                @EsMusica,
                                @EsComercial,
                                @ClaseGenero,
                                @Duracion )",
                             new
                             {
                                ExpedienteId = expedienteId,
                                Partitura = partitura,
                                Letra = letra,
                                Genero = genero,
                                EsAutor = autor,
                                EsMusica = musica,
                                EsComercial = comercial,
                                ClaseGenero = claseogen,
                                Duracion = duracion
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
                    log.Error(string.Format("ObrasMusicalesyEscenicas {0}, {1} tiene errores", idsolicitud, partitura));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END ObrasMusicalesyEscenicas - obrasm


        // Productores - productor.dbf
        public static void ImportProductores()
        {
            string DBF_FileName = "productor";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string nombre = dataReader.GetString(1);

                try
                {
                    int edad = int.Parse(dataReader.GetValue(2).ToString());
                    string estadocivil = dataReader.GetString(3);
                    string profesion = dataReader.GetString(4);
                    string nacionalidad = dataReader.GetString(5);
                    string domicilio = dataReader.GetString(6);
                    string tel = dataReader.GetString(7);
                    string fax = dataReader.GetString(8);
                    string email = dataReader.GetString(9);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();
                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.Productores([ExpedienteId]
                                  ,[Nombre]
                                  ,[Edad]
                                  ,[EstadoCivil]
                                  ,[Profesion]
                                  ,[Nacionalidad]
                                  ,[Domicilio]
                                  ,[Telefono]
                                  ,[Fax]
                                  ,[Email]
                                )
                        VALUES( @ExpedienteId,
                                @Nombre,
                                @Edad,
                                @EstadoCivil,
                                @Profesion,
                                @Nacionalidad,
                                @Domicilio,
                                @Telefono,
                                @Fax,
                                @Email )",
                             new
                             {
                                ExpedienteId = expedienteId,
                                Nombre = nombre,
                                Edad = edad,
                                EstadoCivil = estadocivil, 
                                Profesion = profesion,
                                Nacionalidad = nacionalidad,
                                Domicilio = domicilio,
                                Telefono = tel,
                                Fax = fax,
                                Email = email
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
                    log.Error(string.Format("Productores {0}, {1} tiene errores", idsolicitud, nombre));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } //  END Productores->productor.dbf

        
        // Resoluciones - resol.dbf
        public static void ImportResoluciones()
        {
            string DBF_FileName = "resol";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string idresol = dataReader.GetString(1);

                try
                {
                    DateTime fresol = dataReader.GetDateTime(2);
                    DateTime frechazo = dataReader.GetDateTime(3);
                    DateTime fsuspension = dataReader.GetDateTime(4);
                    DateTime fmemorial = dataReader.GetDateTime(5);
                    string articulos = dataReader.GetString(6);
                    string omitio = dataReader.GetString(7);
                    string referente = dataReader.GetString(8);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();
                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.DAResoluciones([ExpedienteId]
                                  ,[Fecha]
                                  ,[Articulos]
                                  ,[Omitio]
                                  ,[Referente]
                                )
                        VALUES( @ExpedienteId,
                                @Fecha,
                                @Articulos,
                                @Omitio,
                                @Referente )",
                             new
                             {
                                ExpedienteId = expedienteId,
                                Fecha = fresol,
                                Articulos = articulos,
                                Omitio = omitio,
                                Referente = referente
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
                    log.Error(string.Format("resol table {0}, {1} tiene errores", idsolicitud, idresol));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } //  END Resoluciones - resol.dbf

        // Solicitantes - solicitante.dbf
        public static void ImportSolicitantes()
        {
            string DBF_FileName = "solicitante";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string idsolicitud = dataReader.GetValue(0).ToString();
                string nombre = dataReader.GetString(1);

                try
                {
                    int edad = int.Parse(dataReader.GetValue(2).ToString());
                    string estadocivil = dataReader.GetString(3);
                    string profesion = dataReader.GetString(4);
                    string domicilio = dataReader.GetString(5);
                    string nacionalidad = dataReader.GetString(6);
                    string lugarnoti = dataReader.GetString(7);
                    string tel = dataReader.GetString(8);
                    string fax = dataReader.GetString(9);
                    string email = dataReader.GetString(10);
                    string calidad = dataReader.GetString(11);
                    string entidadsol = dataReader.GetString(12);
                    string lugarconst = dataReader.GetString(13);
                    string objetosol = dataReader.GetString(14);
                    string encalidad = dataReader.GetString(15);
                    string derechomediante = dataReader.GetString(16);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE ModuloId=3 AND Numero = @numero", new { numero = idsolicitud }).FirstOrDefault();
                    if (expedienteId > 0)
                    {
                        log.Error("found expediente id");

                        cnn.Execute(@"INSERT INTO da.Solicitantes([ExpedienteId]
                                    ,[Nombre]
                                    ,[Edad]
                                    ,[EstadoCivil]
                                    ,[Profesion]
                                    ,[Domicilio]
                                    ,[Nacionalidad]
                                    ,[LugarNotificacion]
                                    ,[Telefono]
                                    ,[Fax]
                                    ,[Email]
                                    ,[Calidad]
                                    ,[EntidadSolicitante]
                                    ,[LugarConstitucion]
                                    ,[ObjetoSolicitud]
                                    ,[EnCalidad]
                                    ,[AdquirioDerecho]
                                )
                        VALUES( @ExpedienteId,
                                @Nombre,
                                @Edad,
                                @EstadoCivil,
                                @Profesion,
                                @Domicilio,
                                @Nacionalidad,
                                @LugarNotificacion,
                                @Telefono,
                                @Fax,
                                @Email,
                                @Calidad,
                                @EntidadSolicitante,
                                @LugarConstitucion,
                                @ObjetoSolicitud,
                                @EnCalidad,
                                @AdquirioDerecho
                            )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 Nombre = nombre,
                                 Edad = edad,
                                 EstadoCivil = estadocivil,
                                 Profesion = profesion,
                                 Domicilio = domicilio,
                                 Nacionalidad = nacionalidad,
                                 LugarNotificacion = lugarnoti,
                                 Telefono = tel,
                                 Fax = fax,
                                 Email = email,
                                 Calidad = calidad,
                                 EntidadSolicitante = entidadsol,
                                 LugarConstitucion = lugarconst,
                                 ObjetoSolicitud = objetosol,
                                 EnCalidad = encalidad,
                                 AdquirioDerecho = derechomediante
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
                    log.Error(string.Format("Solicitantes {0}, {1} tiene errores", idsolicitud, nombre));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } //  END Productores->productor.dbf

    }
}
