using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataImport
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            string x = "";
            do
            {
                Console.WriteLine("Data import");
                Console.WriteLine("------");
                Console.WriteLine("(1)Marcas     (2)Patentes    (3)Derecho de Autor    ");
                Console.WriteLine("------");
                Console.WriteLine("(x)Terminar");
                x = Console.ReadLine();

                switch (x)
                {
                    case "1":
                        menuMarcas();
                        break;
                    case "2":
                        menuPatentes();
                        break;
                    case "3":
                        menuDA();
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        break;
                }
                
                Console.Clear();
            } while (!x.Equals("x"));


            Console.WriteLine("Proceso de transferencia de datos finalizada...");            
        }

        public static void menuMarcas() 
        {
            Console.Clear();
            string x = "";
            do
            {
                Console.WriteLine("Marcas");
                Console.WriteLine("------");
                Console.WriteLine("(1)Importar todo     (2)Expedientes    (3)Cronologia     (4)Prioridades");
                Console.WriteLine("(5)Titulares         (6) Mandatarios   (7) Anotaciones   (8)Expedientes en anotacion");
                Console.WriteLine("------");
                Console.WriteLine("(x)Terminar");
                x = Console.ReadLine();
                System.Diagnostics.Debug.WriteLine(x);
                switch (x)
                {
                    case "1":
                        todoMarcas();
                        break;
                    case "2":
                        log.Error("Expedientes");
                        Marcas.ImportExpedientes();
                        break;
                    case "3":
                        log.Error("Cronologia");
                        Marcas.ImportCrono();
                        break;
                    case "4":
                        log.Error("Prioridades");
                        Marcas.ImportPrioridades();
                        break;
                    case "5":
                        log.Error("Titulares");
                        Marcas.ImportTITULARES();
                        break;
                    case "6":
                        log.Error("Mandatarios");
                        Marcas.ImportMANDATARIOS();
                        break;
                    case "7":
                        log.Error("Anotaciones");
                        Marcas.ImportAnotaciones();
                        break;
                    case "8":
                        log.Error("Anotacion-Registros");
                        Marcas.ImportAnotacionRegistros();
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        break;
                }

                Console.Clear();
            } while (!x.Equals("x"));
        }

        public static void menuPatentes() {
            Console.Clear();
            string x = "";
            do
            {
                Console.WriteLine("Patentes");
                Console.WriteLine("------");
                Console.WriteLine("(1)Importar todo     (2)Agentes          (3)Expedientes  ");
                Console.WriteLine("(4)Cronologia        (5)Abandono         (6)Anualidades ");
                Console.WriteLine("(7)DoctosEx          (8)Inventores       (9)IPC          ");
                Console.WriteLine("(10)Prioridades      (11)Titulares       (12)Titulares De la Patente");
                Console.WriteLine("(13)Titulos          (14)Resoluciones");
                Console.WriteLine("------");
                Console.WriteLine("(x)Terminar");
                x = Console.ReadLine();

                switch (x)
                {
                    case "1":
                        todoPatentes();
                        break;
                    case "2":
                        log.Error("Agentes");
                        Patentes.ImportAgentes();
                        break;
                    case "3":
                        log.Error("Expedientes de Patentes");
                        Patentes.ImportExpedientes();
                        break;
                    case "4":
                        log.Error("Cronologia de Patentes");
                        Patentes.ImportCrono();
                        break;
                    case "5":
                        log.Error("Pat-Abandono");
                        Patentes.ImportAbandono();
                        break;
                    case "6":
                        log.Error("Pat-Anualidades");
                        Patentes.ImportAnualidades();
                        break;
                    case "7":
                        log.Error("Pat-DoctosExt");
                        Patentes.ImportDoctosExt();
                        break;
                    case "8":
                        log.Error("Pat-Inventores");
                        Patentes.ImportInventores();
                        break;
                    case "9":
                        log.Error("Pat-IPC");
                        Patentes.ImportIPC(); //Create index column from indice char of 2, -- REMOVE SET FILTER TO index="**"
                        break;
                    case "10":
                        log.Error("Pat-Prioridades");
                        Patentes.ImportPrioridades();
                        break;
                    case "11":
                        log.Error("Pat-Titulares");
                        Patentes.ImportTitulares(); //DELETE FOR EMPTY(idtitular)
                        break;
                    case "12":
                        log.Error("Pat-Titulares de la Patente");
                        Patentes.ImportTitularesDeLaPatente();
                        break;
                    case "13":
                        log.Error("Pat-Titulos");
                        Patentes.ImportTitulos();
                        break;
                    case "14":
                        log.Error("Pat-Resoluciones");
                        Patentes.ImportResoluciones();
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        break;
                }

                Console.Clear();
            } while (!x.Equals("x"));
        }

        public static void menuDA() 
        {
            Console.Clear();
            string x = "";
            do
            {
                Console.WriteLine("Derecho de Autor");
                Console.WriteLine("------");
                Console.WriteLine("(1)Importar todo         (2)Expedientes             (3)Literarias y Artisticas ");
                Console.WriteLine("(4)Audio-Autores         (5)Aporte-Audiovisual      (6)Composiciones-Autores    ");
                Console.WriteLine("(7)Guion-Autores         (8)Autores                 (9)Cronologia            ");
                Console.WriteLine("(10)Artistas-Fonograma   (11)Fono-Titulo de Obra    (12)Obras Musicales y Escenicas");
                Console.WriteLine("(13)Productores          (14)Resoluciones           (15)Solicitantes");
                Console.WriteLine("------");
                Console.WriteLine("(x)Terminar");
                x = Console.ReadLine();

                switch (x)
                {
                    case "1":
                        todoDA();
                        break;
                    case "2":
                        log.Error("DA-ImportExpedientes");
                        DerechoDeAutor.ImportExpedientes();
                        break;
                    case "3":
                        log.Error("DA-ImportLiterariasyArtisticas");
                        DerechoDeAutor.ImportLiterariasyArtisticas();
                        break;
                    case "4":
                        log.Error("DA-ImportAudiovisualAutores");
                        DerechoDeAutor.ImportAudiovisualAutores();
                        break;
                    case "5":
                        log.Error("DA-ImportAporteAudiovisual");
                        DerechoDeAutor.ImportAporteAudiovisual();
                        break;
                    case "6":
                        log.Error("DA-ImportComposicionAutores");
                        DerechoDeAutor.ImportComposicionAutores();
                        break;
                    case "7":
                        log.Error("DA-ImportGuionAutores");
                        DerechoDeAutor.ImportGuionAutores();
                        break;
                    case "8":
                        log.Error("DA-ImportAutores");
                        DerechoDeAutor.ImportAutores();
                        break;
                    case "9":
                        log.Error("DA-ImportCrono");
                        DerechoDeAutor.ImportCrono();
                        break;
                    case "10":
                        log.Error("DA-ImportFonogramaArtistas");
                        DerechoDeAutor.ImportFonogramaArtistas();
                        break;
                    case "11":
                        log.Error("DA-ImportFonogramaTituloDeObras");
                        DerechoDeAutor.ImportFonogramaTituloDeObras();
                        break;
                    case "12":
                        log.Error("DA-ImportObrasMusicalesyEscenicas");
                        DerechoDeAutor.ImportObrasMusicalesyEscenicas();
                        break;
                    case "13":
                        log.Error("DA-ImportProductores");
                        DerechoDeAutor.ImportProductores();
                        break;
                    case "14":
                        log.Error("DA-ImportResoluciones");
                        DerechoDeAutor.ImportResoluciones();
                        break;
                    case "15":
                        log.Error("DA-ImportSolicitantes");
                        DerechoDeAutor.ImportSolicitantes();
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        break;
                }

                Console.Clear();
            } while (!x.Equals("x"));
        }


        public static void todoMarcas() {

            ///
            /// MARCAS
            ///

            Marcas.ImportExpedientes();
            Marcas.ImportCrono();
            Marcas.ImportPrioridades();
            Marcas.ImportTITULARES();
            Marcas.ImportMANDATARIOS();
            Marcas.ImportAnotaciones();
            Marcas.ImportAnotacionRegistros();
        }

        public static void todoPatentes() {

            ///
            /// Patentes
            ///
            //Patentes.ImportEstatus();
            //Patentes.ImportClasificaciones();

            Patentes.ImportAgentes();            
            Patentes.ImportExpedientes();
            Patentes.ImportCrono();
            Patentes.ImportAbandono();
            Patentes.ImportAnualidades();
            Patentes.ImportDoctosExt();
            Patentes.ImportInventores();
            Patentes.ImportIPC(); //Create index column from indice char of 2, -- REMOVE SET FILTER TO index="**"
            Patentes.ImportPrioridades();
            Patentes.ImportTitulares(); //DELETE FOR EMPTY(idtitular)
            Patentes.ImportTitularesDeLaPatente();
            Patentes.ImportTitulos(); 
            Patentes.ImportResoluciones();
        }

        public static void todoDA() {
            ///
            /// Derecho de Autor
            ///
            //DerechoDeAutor.ImportEstatus();
            //DerechoDeAutor.ImportFormularios();
            //DerechoDeAutor.ImportTemplates();

            DerechoDeAutor.ImportExpedientes();
            DerechoDeAutor.ImportLiterariasyArtisticas();
            DerechoDeAutor.ImportAudiovisualAutores();
            DerechoDeAutor.ImportAporteAudiovisual();
            DerechoDeAutor.ImportComposicionAutores();
            DerechoDeAutor.ImportGuionAutores();
            DerechoDeAutor.ImportAutores();
            DerechoDeAutor.ImportCrono();
            DerechoDeAutor.ImportFonogramaArtistas();
            DerechoDeAutor.ImportFonogramaTituloDeObras();            
            DerechoDeAutor.ImportObrasMusicalesyEscenicas();
            DerechoDeAutor.ImportProductores();
            DerechoDeAutor.ImportResoluciones();
            DerechoDeAutor.ImportSolicitantes();

        }

    }
}
