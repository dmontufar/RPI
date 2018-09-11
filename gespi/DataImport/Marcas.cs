using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Timers;
using System.Threading;
using System.Configuration;

namespace DataImport
{
    public class Marcas
    {
        //public const string ConnectionString = @"Data Source=MANOLO\SQLEXPRESS;Initial Catalog=GPI;Integrated Security=False;User ID=express;Password=3xpr3ss;MultipleActiveResultSets=True;";
        public static string ConnectionString;
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

        public class productos
        {
            public string reservas { get; set; }
            public string prods { get; set; }
            public string grafica { get; set; }
        }

        public static productos getProductos(string expediente)
        {
            string FilePath = ConfigurationManager.AppSettings["legacyDb"];

            var cnn = GetOpenConnection();

            string DBF_FileName = "produc.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");
            conn.Open();

            var products = new productos();
            using (OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE NRO_SOLIC =" + expediente, conn))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        products.reservas = dataReader.GetString(2);
                        products.prods = dataReader.GetString(3);
                        products.grafica = dataReader.GetString(4);
                        break;
                    }

                }
            }
            return products;
        }

        public static void ImportExpedientes()
        {
            string FilePath = ConfigurationManager.AppSettings["legacyDb"];

            var cnn = GetOpenConnection();

            string DBF_FileName = "MARCA.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE NRO_SOLIC >201610564", conn);
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string nro_solic = dataReader.GetValue(0).ToString();
                try
                {
                    string f_solic = dataReader.GetString(1);
                    string tip_reg = dataReader.GetString(2);
                    string nro_reg = dataReader[3].ToString();
                    string f_status = dataReader.GetString(5);
                    string status = dataReader.GetString(6);
                    string tip_activi = dataReader.GetString(7);
                    string ley = dataReader["Ley"].ToString();
                    string no_time = dataReader["No_time"].ToString();
                    string tip_marca = dataReader["Tip_marca"].ToString();
                    string nom_marca = dataReader["Nom_marca"].ToString();
                    string traducida = dataReader["Traducida"].ToString();
                    bool industrial = dataReader["Activi1"] as int? == 1 ? true : false;
                    bool deServicios = dataReader["Activi2"] as int? == 1 ? true : false;
                    bool comercial = dataReader["Activi3"] as int? == 1 ? true : false;
                    bool certificacion = dataReader["Activi4"] as int? == 1 ? true : false;
                    bool colectiva = dataReader["Activi5"] as int? == 1 ? true : false;
                    bool activi6 = dataReader["Activi6"] as int? == 1 ? true : false;
                    //int? registro = dataReader["nro_Reg"] as int?;
                    //string nro_reg = dataReader["nro_Reg"].ToString();
                    int registro = parseToInt(nro_reg);

                    string raya = dataReader["Raya"].ToString();
                    int? tomo = dataReader["No_tomo"] as int?;
                    int? folio = dataReader["No_folio"] as int?;
                    string doctosAdjuntos = string.Empty;
                    doctosAdjuntos += dataReader["Op1"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op2"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op3"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op4"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op5"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op6"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op7"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op8"].ToString() + ",";
                    string otrosDoctosAdjuntos = dataReader["Otros"].ToString();
                    string ubicacion = dataReader["Ubicacion"].ToString();
                    string ult_renov = dataReader["Ult_renov"].ToString();
                    string ext_marca = dataReader["Ext_marca"].ToString();
                    int? PaisConstituidaId = null;
                    string UbicacionActual = dataReader["Ubicacion"].ToString();
                    string clases = dataReader["Clases"].ToString();

                    int leyId = (ley == "N" ? 1 : 2);
                    var timex = no_time.Split(':').ToList<string>();
                    if (timex.Count() == 2) timex.Add("");

                    DateTime fecha_estatus = (string.IsNullOrEmpty(f_status.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(f_status.Substring(0, 4)), int.Parse(f_status.Substring(4, 2)), int.Parse(f_status.Substring(6, 2))));

                    DateTime fecha_solicitud = (string.IsNullOrEmpty(f_solic.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(f_solic.Substring(0, 4)), int.Parse(f_solic.Substring(4, 2)), int.Parse(f_solic.Substring(6, 2))));

                    TimeSpan time = (timex.Count() < 2) ? new TimeSpan(0, 0, 0) : new TimeSpan(parseToInt(timex[0]), parseToInt(timex[1]), parseToInt(timex[2]));

                    int found = cnn.Query<int>("select Id FROM dbo.Expedientes WHERE Numero = @nro_solic AND moduloId=1", new { nro_solic = nro_solic }).FirstOrDefault();
                    if (found > 0)
                    {
                        // UPDATE numero de registro
                        log.Error(string.Format("REGISTRO: {0} -- {1}", registro, nro_reg));
                        log.Error(string.Format("Expediente {0} ya existe en la base de datos:", nro_solic));
                        cnn.Execute(@"UPDATE dbo.Marcas SET registro = @nro_reg, Raya = @raya WHERE ExpedienteId=@expedienteId",
                            new
                            {
                                nro_reg = registro,
                                expedienteId = found,
                                raya = raya
                            }
                        );

                        continue;
                    }


                    var p = getProductos(nro_solic);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status }).FirstOrDefault();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE codigo like @codigo", new { codigo = tip_reg + "%" }).FirstOrDefault();

                    int tipoDeMarca = cnn.Query<int>("select Id FROM dbo.TiposDeMarca WHERE flag like @flag", new { flag = tip_marca + "%" }).FirstOrDefault();

                    int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId])
                    VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId);
                    SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                        new
                        {
                            ModuloId = 1,
                            TipoDeRegistroId = tipoDeRegistroId,
                            Numero = nro_solic,
                            FechaDeSolicitud = fecha_solicitud,
                            Hora = time,
                            EstatusId = statusId,
                            FechaDeEstatus = fecha_estatus,
                            LeyId = leyId
                        }
                        ).Single();

                    int classificacionDeNizaId = cnn.Query<int>("select Id FROM dbo.ClassificacionDeNiza WHERE Codigo like @codigo", new { codigo = clases }).FirstOrDefault();

                    cnn.Execute(@"INSERT INTO dbo.Marcas([ExpedienteId]
                            ,[Recibo]
                            ,[TipoDeMarca]
                            ,[Denominacion]
                            ,[Traduccion]
                            ,[Industrial]
                            ,[DeServicios]
                            ,[Comercial]
                            ,[Certificacion]
                            ,[Colectiva]
                            ,[Registro]
                            ,[Raya]
                            ,[Tomo]
                            ,[Folio]
                            ,[ClassificacionDeNizaId]
                            ,[Productos]
                            ,[Reservas]
                            ,[DescripcionGrafica]
                            ,[DoctosAdjuntos]
                            ,[OtrosDoctosAdjuntos]
                            ,[CaracteristicasCom]
                            ,[EstandaresDeCalidad]
                            ,[AutoridadApReglamento]
                            ,[DireccionComercializacion]
                            ,[UltimaRenovacion]
                            ,[ExtensionDeLaMarca]
                            ,[PaisConstituidaId]
                            ,[UbicacionActual]
                            ,[UbicacionAnterior]
                            ,[FechaDeTraslado]
                            ,[MotivoDeTraslado])
                        VALUES(@ExpedienteId,
                        @Recibo,
                        @TipoDeMarca,
                        @Denominacion,
                        @Traduccion,
                        @Industrial,
                        @DeServicios,
                        @Comercial,
                        @Certificacion,
                        @Colectiva,
                        @Registro,
                        @Raya,
                        @Tomo,
                        @Folio,
                        @ClassificacionDeNizaId,
                        @Productos,
                        @Reservas,
                        @DescripcionGrafica,
                        @DoctosAdjuntos,
                        @OtrosDoctosAdjuntos,
                        @CaracteristicasCom,
                        @EstandaresDeCalidad,
                        @AutoridadApReglamento,
                        @DireccionComercializacion,
                        @UltimaRenovacion,
                        @ExtensionDeLaMarca,
                        @PaisConstituidaId,
                        @UbicacionActual,
                        @UbicacionAnterior,
                        @FechaDeTraslado,
                        @MotivoDeTraslado
                        )",
                         new
                         {
                             ExpedienteId = expedienteId,
                             Recibo = "",
                             TipoDeMarca = tipoDeMarca,
                             Denominacion = nom_marca,
                             Traduccion = traducida,
                             Industrial = industrial,
                             DeServicios = deServicios,
                             Comercial = comercial,
                             Certificacion = certificacion,
                             Colectiva = colectiva,
                             Registro = registro,
                             Raya = raya,
                             Tomo = tomo,
                             Folio = folio,
                             ClassificacionDeNizaId = classificacionDeNizaId,
                             Productos = p.prods,
                             Reservas = p.reservas,
                             DescripcionGrafica = p.grafica,
                             DoctosAdjuntos = doctosAdjuntos,
                             OtrosDoctosAdjuntos = otrosDoctosAdjuntos,
                             CaracteristicasCom = "[pendiente]",
                             EstandaresDeCalidad = "[pendiente]",
                             AutoridadApReglamento = "[pendiente]",
                             DireccionComercializacion = ubicacion,
                             UltimaRenovacion = ult_renov,
                             ExtensionDeLaMarca = ext_marca,
                             PaisConstituidaId = PaisConstituidaId,
                             UbicacionActual = "[pendiente]",
                             UbicacionAnterior = "[pendiente]",
                             FechaDeTraslado = new DateTime(1900, 1, 1),
                             MotivoDeTraslado = "[pendiente]"
                         }
                        );
                    log.Error(string.Format("Expediente {0} importado:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0} has errors:", nro_solic));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        public static int parseToInt(string value)
        {
            var result = 0;
            try
            {
                if (!string.IsNullOrEmpty(value.Trim()))
                    result = int.Parse(value);
            }
            catch
            {
                log.Error(string.Format("Convertion error {0}:", value));
            }
            return result;
        }

        public static void ImportCrono()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "evento.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string nro_solic = dataReader.GetValue(0).ToString();
                int usuarioId = 0;
                try
                {
                    string f_evento = dataReader.GetString(1);
                    string status = dataReader.GetString(3);
                    string referencia = dataReader.GetString(4);
                    string persona = dataReader.GetString(5);
                    string observaciones = dataReader.GetString(7);
                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE Numero = @numero", new { numero = nro_solic }).FirstOrDefault();

                    DateTime fecha = (string.IsNullOrEmpty(f_evento.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(f_evento.Substring(0, 4)), int.Parse(f_evento.Substring(4, 2)), int.Parse(f_evento.Substring(6, 2))));

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status }).FirstOrDefault();

                    if (expedienteId > 0)
                    {
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
                                 UsuarioId = usuarioId,
                                 Observaciones = observaciones,
                                 UsuarioIniciales = persona
                             }
                            );
                    }
                    else
                    {
                        log.Error(nro_solic);

                        // expedientes inexistentes 
                        //198500055
                        //199900000
                        //199900000
                        //197000001
                        //199500000
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Cronologia del expediente Expediente {0} tiene errores:", nro_solic));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        //
        //DELETE FOR EMPTY(FECHA) AND EMPTY(PAIS) AND EMPTY(NUMERO)
        //PACK
        public static void ImportPrioridades()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "priorida.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string nro_solic = dataReader.GetValue(0).ToString();
                string numero = dataReader.GetString(3);
                try
                {
                    DateTime? fecha = dataReader.GetDateTime(1); //date
                    string pais = dataReader.GetString(2);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE Numero = @numero", new { numero = nro_solic }).FirstOrDefault();
                    if (expedienteId == 0)
                    {
                        log.Error(string.Format("Expediente {0} no encontrado, prioridad numero: {1}", nro_solic, numero));
                        continue;
                    }
                    if (fecha == null)
                    {
                        log.Error(string.Format("Invalid Fecha Expediente {0} no encontrado, prioridad numero: {1}", nro_solic, numero));
                        continue;
                    }

                    //DateTime fecha = (string.IsNullOrEmpty(f_evento.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(f_evento.Substring(0, 4)), int.Parse(f_evento.Substring(4, 2)), int.Parse(f_evento.Substring(6, 2))));

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();


                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"INSERT INTO dbo.PrioridadMarcas([ExpedienteId]
                            ,[Fecha]
                            ,[PaisId]
                            ,[Numero])
                        VALUES(@ExpedienteId,
                            @Fecha,
                            @PaisId,
                            @Numero
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 Fecha = fecha,
                                 PaisId = paisId,
                                 Numero = numero
                             }
                            );
                    }
                    else
                    {
                        log.Error(nro_solic);
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Prioridad del Expediente: {0} tiene errores numero:{1}", nro_solic, numero));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


        public class Titular
        {
            public string nombre { get; set; }
        }

        private static Titular getTitular(string nro_tit)
        {
            string FilePath = ConfigurationManager.AppSettings["legacyDb"];

            string DBF_FileName = "titular.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");
            conn.Open();

            var titular = new Titular();
            using (OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE nro_tit =" + nro_tit, conn))
            {
                using (var dataReader2 = command.ExecuteReader())
                {
                    while (dataReader2.Read())
                    {
                        titular.nombre = dataReader2.GetString(1);
                        break;
                    }

                }
            }
            return titular;
        }



        //
        //DELETE FOR EMPTY(FECHA) AND EMPTY(PAIS) AND EMPTY(NUMERO)
        //PACK
        public static void ImportTITULARES()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "titumar.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE NRO_SOLIC >201410564", conn);
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            int counter = 0;
            while (dataReader.Read())
            {
                //log.Error(string.Format("record: {0}", ++counter));
                string nro_solic = dataReader.GetValue(0).ToString();
                string OldId = dataReader.GetValue(1).ToString();
                try
                {
                    string dir_noti = dataReader.GetString(2);
                    string dir_ubi = dataReader.GetString(3);
                    string pais = dataReader.GetString(4);

                    var titular = getTitular(OldId);

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE Numero = @numero", new { numero = nro_solic }).FirstOrDefault();
                    if (expedienteId == 0)
                    {
                        log.Error(string.Format("Expediente {0} no encontrado, titular numero: {1}", nro_solic, OldId));
                        continue;
                    }

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();


                    var titularId = InsertTitular(cnn, titular.nombre, dir_ubi, paisId, int.Parse(OldId));

                    if (titularId > 0)
                    {

                        cnn.Execute(@"INSERT INTO dbo.[TitularesDeLaMarca]([ExpedienteId]
                            ,[TitularId]
                            ,[DireccionParaNotificacion]
                            ,[DireccionParaUbicacion]
                            ,[EnCalidadDe]
                            )
                        VALUES(@ExpedienteId,
                            @TitularId,
                            @DireccionParaNotificacion,
                            @DireccionParaUbicacion,
                            @EnCalidadDe
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 TitularId = titularId,
                                 DireccionParaNotificacion = dir_noti,
                                 DireccionParaUbicacion = dir_ubi,
                                 EnCalidadDe = ""
                             }
                            );
                    }
                    else
                    {
                        log.Error(string.Format("Expediente: {0} tiene errores titular numero:{1}", nro_solic, OldId));
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente: {0} tiene errores titular numero:{1}", nro_solic, OldId));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        private static int InsertTitular(SqlConnection cnn, string nombre, string dir, int paisId, int oldId)
        {
            int titularId = 0;
            try
            {
                titularId = cnn.Query<int>("SELECT Id FROM [dbo].[Titulares] WHERE Nombre = @nombre AND Direccion = @direccion", new { nombre = nombre, direccion = dir }).FirstOrDefault();
                if (titularId == 0) { 
                    //si el titular ya existe no lo agregamos
                    titularId = cnn.Query<int>(@"INSERT INTO dbo.[Titulares]([Nombre]
                                  ,[Direccion]
                                  ,[PaisId]
                                  ,[OldId]
                                )
                            SELECT @Nombre, @Direccion, @PaisId, @OldId
                            WHERE NOT EXISTS(
                                SELECT 1
                                FROM dbo.[Titulares]
                                WHERE OldId = @OldId
                            );
                            SELECT Id AS [Id]
                            FROM dbo.[Titulares]
                            WHERE OldId = @OldId;",
                         new
                         {
                             Nombre = nombre,
                             Direccion = dir,
                             PaisId = paisId,
                             OldId = oldId
                         }
                        ).Single();
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Expediente: tiene errores titular numero:{0}", oldId));
                log.Error(e.Message);
            }

            return titularId;
        }



        //
        // MANDATARIOS
        //

        public static void ImportMANDATARIOS()
        {
            if (true)
                return; //ERROR en la logica de esta API

            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "MANDATAR.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE NRO_MANDAT  >201410564", conn);
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            int counter = 0;
            while (dataReader.Read())
            {
                log.Error(string.Format("record: {0}", ++counter));
                string nro_solic = dataReader.GetValue(0).ToString();
                string nom_mandat = dataReader.GetString(1);

                try
                {

                    int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE Numero = @numero", new { numero = nro_solic }).FirstOrDefault();
                    if (expedienteId == 0)
                    {
                        log.Error(string.Format("Expediente {0} no encontrado, mandatario: {1}", nro_solic, nom_mandat));
                        continue;
                    }


                    var mandatarioId = InsertMandatario(cnn, nom_mandat);

                    if (mandatarioId > 0)
                    {

                        cnn.Execute(@"INSERT INTO dbo.[MandatarioDeLaMarca]([ExpedienteId]
                            ,[MandatarioId]
                            )
                        VALUES(@ExpedienteId,
                            @MandatarioId
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 MandatarioId = mandatarioId
                             }
                            );
                    }
                    else
                    {
                        log.Error(string.Format("Expediente {0} con errores, mandatario: {1}", nro_solic, nom_mandat));
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0} con errores: {1}", nro_solic, nom_mandat));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        private static int InsertMandatario(SqlConnection cnn, string nombre)
        {
            int Id = 0;
            try
            {

                Id = cnn.Query<int>(@"INSERT INTO dbo.[Mandatarios]([Nombre])
                        SELECT @Nombre
                        WHERE NOT EXISTS(
                            SELECT 1
                            FROM dbo.[Mandatarios]
                            WHERE Nombre = @Nombre
                        );
                        SELECT Id AS [Id]
                        FROM dbo.[Mandatarios]
                        WHERE Nombre = @Nombre;",
                     new
                     {
                         Nombre = nombre.Trim()
                     }
                    ).Single();
            }
            catch (Exception e)
            {
                log.Error(string.Format("Expediente: tiene errores mandatario:{0}", nombre));
                log.Error(e.Message);
            }

            return Id;
        }



        //
        // DELETE FOR EMPTY(FECHA) AND EMPTY(PAIS) AND EMPTY(NUMERO)
        // PACK
        // DELETE FOR EMPTY(f_solic) AND EMPTY(nro_solic)
        // DELETE FOR EMPTY(tip_anota)

        /*
          DELETE FROM anota WHERE EMPTY(tip_anota)
          DELETE from ANOTA WHERE tip_anota='RE'
          PACK
         */
        public static void ImportAnotaciones()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "anota.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string nro_solic = dataReader.GetValue(0).ToString();
                string tip_anota = dataReader.GetValue(1).ToString();
                string f_solic = dataReader.GetString(2);
                string status = dataReader.GetValue(3).ToString();
                string nom_tit = dataReader.GetValue(4).ToString();
                string pai_tit = dataReader.GetValue(5).ToString();
                string f_status = dataReader.GetString(6);
                string ley = dataReader.GetValue(8).ToString();
                string nro_tit = dataReader.GetValue(9).ToString();
                string nro_ag = dataReader.GetValue(10).ToString();
                string dir_tit = dataReader.GetValue(11).ToString();
                TimeSpan time = new TimeSpan(0, 0, 0);

                try
                {

                    if (tip_anota == "RE" || tip_anota == "")
                    {
                        continue;
                    }

                    int tipoDeRegistroId = 0;
                    string status_code = tip_anota + '-' + status;

                    if (tip_anota.Equals("CN"))
                    {
                        tipoDeRegistroId = 31;
                    }
                    else if (tip_anota.Equals("CA"))
                    {
                        tipoDeRegistroId = 28;
                    }
                    else if (tip_anota.Equals("LI"))
                    {
                        tipoDeRegistroId = 32;
                    }
                    else if (tip_anota.Equals("TR"))
                    {
                        tipoDeRegistroId = 34;
                    }

                    int leyId = 1;
                    if (ley.Equals("N"))
                        leyId = 2;

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status_code }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pai_tit }).FirstOrDefault();

                    DateTime fecha_solicitud;
                    DateTime fecha_estatus;

                    try
                    {
                        fecha_solicitud = (string.IsNullOrEmpty(f_solic.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(f_solic.Substring(0, 4)), int.Parse(f_solic.Substring(4, 2)), int.Parse(f_solic.Substring(6, 2))));
                        fecha_estatus = (string.IsNullOrEmpty(f_status.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(f_status.Substring(0, 4)), int.Parse(f_status.Substring(4, 2)), int.Parse(f_status.Substring(6, 2))));
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Expediente {0} tiene errores en fechas :{1}, {2}", nro_solic, f_solic, f_status));
                        log.Error(e.Message);
                        continue;
                    }

                    Console.WriteLine("STEP (0)");
                    int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId])
                    VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId);
                    SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                        new
                        {
                            ModuloId = 4,
                            TipoDeRegistroId = tipoDeRegistroId,
                            Numero = nro_solic,
                            FechaDeSolicitud = fecha_solicitud,
                            Hora = time,
                            EstatusId = statusId,
                            FechaDeEstatus = fecha_estatus,
                            LeyId = leyId
                        }
                    ).Single();

                    Console.WriteLine("STEP (1)");

                    int titularId = cnn.Query<int>("SELECT Id FROM dbo.Titulares WHERE OldId = @oldTitularId", new { oldTitularId = nro_tit }).FirstOrDefault();

                    Console.WriteLine("STEP (2)");
                    if (titularId == 0)
                        titularId = InsertTitular(cnn, nom_tit, dir_tit, paisId, int.Parse(nro_tit));

                    log.Error("STEP (3)");
                    int AnotacionesTitularId = cnn.Query<int>(@"INSERT INTO dbo.Anotaciones([ExpedienteId],[TitularId],[NuevoTitular],[Direccion],[PaisId])
                    VALUES( @ExpedienteId, @TitularId, @NuevoTitular, @Direccion, @PaisId);
                    SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                            new
                            {
                                ExpedienteId = expedienteId,
                                TitularId = titularId,
                                NuevoTitular = nom_tit,
                                Direccion = dir_tit,
                                PaisId = paisId
                            }
                    ).Single();

                    //FindAndImportAnotacionRegistros(cnn, expedienteId, statusId, nro_solic);                    
                }
                catch (Exception e)
                {
                    log.Error(string.Format("(ImportAnotaciones) Expediente tiene errores :{0}", nro_solic));
                    log.Error(string.Format("(ImportAnotaciones) {0}, {1}, {2}, {3}, {4}, {5}", tip_anota, nro_solic, f_solic, time, status, f_status, ley));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


        public static void ImportAnotacionRegistros()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "anotreg.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            //OleDbCommand command = new OleDbCommand("SELECT ar.*, a.tip_anota as tipo, a.status FROM " + DBF_FileName + " ar INNER JOIN anota.dbf a ON ar.nro_solic = a.nro_solic WHERE ar.nro_solic in (SELECT nro_solic from anota WHERE nro_solic > 20120000 GROUP BY nro_solic HAVING count(*)=1)", conn);
            OleDbCommand command = new OleDbCommand("SELECT ar.*, a.tip_anota as tipo, a.status FROM " + DBF_FileName + " ar INNER JOIN anota.dbf a ON ar.nro_solic = a.nro_solic WHERE ar.nro_solic in (SELECT nro_solic from anota GROUP BY nro_solic HAVING count(*)>=1)", conn);

            conn.Open();
            var dataReader = command.ExecuteReader();
            int counter = 0;
            while (dataReader.Read())
            {
                Console.WriteLine(string.Format("record: {0}", ++counter));

                string nro_solic = dataReader.GetValue(0).ToString();
                string nro_reg = dataReader.GetValue(1).ToString();
                string tip_reg = dataReader.GetString(2);
                string raya = dataReader.GetString(3);
                string tipoAnotacion = dataReader.GetString(5); 
                try
                {

                    string status = dataReader.GetValue(6).ToString();

                    if (tipoAnotacion == "RE" || tipoAnotacion == "")
                    {
                        continue;
                    }

                    int tipoDeRegistroId = 0;

                    if (tipoAnotacion.Equals("CN"))
                    {
                        tipoDeRegistroId = 31;
                    }
                    else if (tipoAnotacion.Equals("CA"))
                    {
                        tipoDeRegistroId = 28;
                    }
                    else if (tipoAnotacion.Equals("LI"))
                    {
                        tipoDeRegistroId = 32;
                    }
                    else if (tipoAnotacion.Equals("TR"))
                    {
                        tipoDeRegistroId = 34;
                    }

                    string status_code = tipoAnotacion + '-' + status;

                    int expedienteAnotacion = cnn.Query<int>("SELECT id FROM Expedientes e WHERE e.numero = @nro_solic AND e.TipoDeRegistroId = @tipoDeRegistroId", new { nro_solic = nro_solic, tipoDeRegistroId = tipoDeRegistroId }).FirstOrDefault();

                    if (expedienteAnotacion == 0)
                    {
                        log.Error(string.Format("Expediente de anotaciones no encontrado, anotreg: {0} {1} {2}", nro_solic, tipoAnotacion, tipoDeRegistroId));
                        continue;
                    }

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status_code }).FirstOrDefault();
                    if (statusId == 0)
                    {
                        log.Error(string.Format("Estatus en anotreg invalido: {0} {1}", nro_solic, tipoAnotacion));
                        continue;
                    }

                    int expedienteDeMarcasId = cnn.Query<int>("SELECT ExpedienteId FROM dbo.Marcas WHERE registro = @registro AND raya=@raya", new { registro = nro_reg, raya = raya.Trim() }).FirstOrDefault();
                    if (expedienteDeMarcasId == 0)
                    {
                        log.Error(string.Format("Expediente no encontrado, anotreg: {0}, {1}, {2}", nro_solic, nro_reg, raya));
                        continue;
                    }


                    cnn.Execute(@"INSERT INTO [dbo].[AnotacionEnExpedientes](
                                [ExpedienteId]
                               ,[AnotacionEnRegistro]
                               ,[Raya]
                               ,[AnotacionEnExpedienteId]
                               ,[EstatusId]
                               )
                        VALUES(@ExpedienteId,
                            @AnotacionEnRegistro,
                            @Raya,
                            @AnotacionEnExpedienteId,
                            @EstatusId
                    )",
                        new
                        {
                            ExpedienteId = expedienteAnotacion,
                            AnotacionEnRegistro = nro_reg,
                            Raya = raya,
                            AnotacionEnExpedienteId = expedienteDeMarcasId,
                            EstatusId = statusId
                        }
                    );
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente en anotreg {0}, {1} tiene errores", nro_solic, tipoAnotacion));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


        //separate this !!
        private static void FindAndImportAnotacionRegistros(SqlConnection cnn, int expedienteId, int statusId, string oldExpediente)
        {

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "anotreg.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE nro_solic =" + oldExpediente, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            int counter = 0;
            while (dataReader.Read())
            {
                log.Error(string.Format("record: {0}", ++counter));

                string nro_solic = dataReader.GetValue(0).ToString();
                string nro_reg = dataReader.GetValue(1).ToString();
                string tip_reg = dataReader.GetString(2);
                string raya = dataReader.GetString(3);

                int expedienteDeMarcasId = cnn.Query<int>("SELECT ExpedienteId FROM dbo.Marcas WHERE registro = @registro AND raya=@raya", new { registro = nro_reg, raya = raya }).FirstOrDefault();


                if (expedienteDeMarcasId == 0)
                {
                    log.Error(string.Format("Expediente no encontrado, anotreg: {0}", nro_solic));
                    continue;
                }

                try
                {
                    cnn.Execute(@"INSERT INTO dbo.Anotaciones(
                             [ExpedienteId]
                            ,[AnotacionEnRegistro]
                            ,[Raya]
                            ,[AnotacionEnExpedienteId]
                            ,[EstatusId]
                        )
                        VALUES(@ExpedienteId,
                            @AnotacionEnRegistro,
                            @Raya,
                            @AnotacionEnExpedienteId,
                            @EstatusId
                    )",
                        new
                        {
                            ExpedienteId = expedienteId,
                            AnotacionEnRegistro = nro_reg,
                            Raya = raya,
                            AnotacionEnExpedienteId = expedienteDeMarcasId,
                            EstatusId = statusId
                        }
                    );
                }
                catch (Exception e)
                {
                    log.Error(string.Format("anotreg.dbf Expediente: {0} tiene errores", oldExpediente));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


    }
}
