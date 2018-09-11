using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

using Common.Web;
using PI.Baktun.Areas.Admin.Controllers;
//using PI.Baktun.Areas.Tmp.Controllers;
using PI.Baktun.Controllers;
using PI.Core;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;

namespace PI.Baktun
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SetUpAutoFac();
        }

        void SetUpAutoFac()
        {
            System.Diagnostics.Trace.TraceError("Autofac Setup...");

            var builder = new ContainerBuilder();
            //
            // REGISTERING CONTROLLERS
            //
            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerRequest();
            builder.RegisterType<CoreController>();
            builder.RegisterType<LoginController>();
            builder.RegisterType<PaisesController>();
            builder.RegisterType<EstatusController>();
            builder.RegisterType<NizaController>();
            builder.RegisterType<ViennaController>();
            builder.RegisterType<MarcaController>();
            builder.RegisterType<PatenteController>();
            builder.RegisterType<DAutorController>();
            builder.RegisterType<ExpedienteController>();
            builder.RegisterType<EventosController>();
            builder.RegisterType<OpsXEventoController>();
            builder.RegisterType<OpcionesController>();
            builder.RegisterType<LeyesController>();
            builder.RegisterType<RolesController>();
            builder.RegisterType<UsuariosController>();
            builder.RegisterType<UsuariosPublicosController>();
            builder.RegisterType<EntitiesController>();
            builder.RegisterType<GacetaController>();
            builder.RegisterType<FavoritosController>();
            builder.RegisterType<AuthController>();
            builder.RegisterType<AuthPublicController>();
            builder.RegisterType<PatTitularController>();
            builder.RegisterType<AgentesController>();
            builder.RegisterType<InventoresController>();
            builder.RegisterType<PermisosController>();
            builder.RegisterType<AnotacionController>();

            //
            // REGISTERING MANAGERS
            //
            builder.RegisterType<UserManager>().As<IUserManager>();
            builder.RegisterType<PaisManager>().As<IPaisManager>();
            builder.RegisterType<EstatusManager>().As<IEstatusManager>();
            builder.RegisterType<EnumManager>().As<IEnumManager>();

            builder.RegisterType<NizaManager>().As<INizaManager>();
            builder.RegisterType<VienaManager>().As<IVienaManager>();
            builder.RegisterType<EventoManager>().As<IEventoManager>();
            builder.RegisterType<OpcionManager>().As<IOpcionManager>();
            builder.RegisterType<ExpedienteManager>().As<IExpedienteManager>();
            builder.RegisterType<MarcaManager>().As<IMarcaManager>();
            builder.RegisterType<OpcionesPorEventoManager>().As<IOpcionesPorEventoManager>();
            builder.RegisterType<LeyesManager>().As<ILeyesManager>();
            builder.RegisterType<RolManager>().As<IRolManager>();

            builder.RegisterType<SessionManager>().As<ISessionManager>();

            builder.RegisterType<UsuarioPublicoManager>().As<IUsuarioPublicoManager>();
            builder.RegisterType<PublicacionManager>().As<IPublicacionManager>();
            builder.RegisterType<FavoritoManager>().As<IFavoritoManager>();
            builder.RegisterType<GrupoManager>().As<IGrupoManager>();
            builder.RegisterType<GrupoExpedienteManager>().As<IGrupoExpedienteManager>();
            builder.RegisterType<GrupoMiembroManager>().As<IGrupoMiembroManager>();
            builder.RegisterType<UsuarioPublicoManager>().As<IUsuarioPublicoManager>();
            builder.RegisterType<EstadisticaManager>().As<IEstadisticaManager>();
            builder.RegisterType<PatTitularManager>().As<IPatTitularManager>();
            builder.RegisterType<PatenteManager>().As<IPatenteManager>();
            builder.RegisterType<AgenteManager>().As<IAgenteManager>();
            builder.RegisterType<InventorManager>().As<IInventorManager>();
            builder.RegisterType<DerechoDeAutorManager>().As<IDerechoDeAutorManager>();
            builder.RegisterType<PermisoManager>().As<IPermisoManager>();


            //
            // REGISTERING REPOSITORIES
            //
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<PaisRepository>().As<IPaisRepository>();
            builder.RegisterType<EstatusRepository>().As<IEstatusRepository>();
            builder.RegisterType<EnumRepository>().As<IEnumRepository>();

            builder.RegisterType<NizaRepository>().As<INizaRepository>();
            builder.RegisterType<VienaRepository>().As<IVienaRepository>();
            builder.RegisterType<EventoRepository>().As<IEventoRepository>();
            builder.RegisterType<OpcionRepository>().As<IOpcionRepository>();
            builder.RegisterType<ExpedienteRepository>().As<IExpedienteRepository>();
            builder.RegisterType<MarcaRepository>().As<IMarcaRepository>();
            builder.RegisterType<OpcionesPorEventoRepository>().As<IOpcionesPorEventoRepository>();
            builder.RegisterType<LeyesRepository>().As<ILeyesRepository>();
            builder.RegisterType<RolRepository>().As<IRolRepository>();
            builder.RegisterType<SessionRepository>().As<ISessionRepository>();
            builder.RegisterType<UsuarioPublicoRepository>().As<IUsuarioPublicoRepository>();
            builder.RegisterType<PublicacionRepository>().As<IPublicacionRepository>();
            builder.RegisterType<FavoritoRepository>().As<IFavoritoRepository>();
            builder.RegisterType<GrupoRepository>().As<IGrupoRepository>();
            builder.RegisterType<GrupoExpedienteRepository>().As<IGrupoExpedienteRepository>();
            builder.RegisterType<GrupoMiembroRepository>().As<IGrupoMiembroRepository>();
            builder.RegisterType<UsuarioPublicoRepository>().As<IUsuarioPublicoRepository>();
            builder.RegisterType<EstadisticaRepository>().As<IEstadisticaRepository>();
            builder.RegisterType<PatTitularRepository>().As<IPatTitularRepository>();
            builder.RegisterType<PatenteRepository>().As<IPatenteRepository>();
            builder.RegisterType<AgenteRepository>().As<IAgenteRepository>();
            builder.RegisterType<InventorRepository>().As<IInventorRepository>();
            builder.RegisterType<DerechoDeAutorRepository>().As<IDerechoDeAutorRepository>();
            builder.RegisterType<PermisoRepository>().As<IPermisoRepository>();
            builder.RegisterType<AuditoriaRepository>().As<IAuditoriaRepository>();

            builder.RegisterType<Repository<Usuario>>().As<IRepository<Usuario>>();
            builder.RegisterType<CryptoManager>().As<ICryptoManager>();
            builder.RegisterType<Transaction>().As<ITransaction>();


            builder.Register(c =>
                new HttpContextWrapper(HttpContext.Current) as HttpContextBase)
                .InstancePerRequest();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerRequest();

            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerRequest();

            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerRequest();

            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerRequest();


            //builder.Register(c => 
            //    //register FakeHttpContext when HttpContext is not available
            //    HttpContext.Current != null ?
            //    (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
            //    (new FakeHttpContext("~/") as HttpContextBase))
            //    .As<HttpContextBase>()
            //    .InstancePerHttpRequest();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacDependencyResolver(container);


            // config.DependencyResolver = new AutoFacContainer(resolver);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();            
            System.Diagnostics.Trace.TraceError(String.Format("{0}{1} - error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), exception.Message));
        }

        public void FixEfProviderServicesProblem()
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }

}
