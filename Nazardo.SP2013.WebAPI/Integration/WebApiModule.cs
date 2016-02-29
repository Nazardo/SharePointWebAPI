using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Microsoft.SharePoint.Administration;

namespace Nazardo.SP2013.WebAPI.Integration
{
    /// <summary>
    /// HttpModule that initialize the WebApi endpoint.
    /// 
    /// This modules tries to mimic the Application_Start method by
    /// providing two entrypoint:
    ///  - OnInit (called each time a process is started)
    ///  - OnStart (called once when the application is started)
    /// </summary>
    public sealed class WebApiModule : IHttpModule
    {
        public static volatile bool HasApplicationStarted = false;
        private readonly object _applicationStartLock = new object();

        public void Init(HttpApplication httpApplication)
        {
            if (HasApplicationStarted == false)
            {
                lock (_applicationStartLock)
                {
                    if (HasApplicationStarted == false)
                    {
                        this.OnStart(httpApplication);
                        HasApplicationStarted = true;
                    }
                }
            }
            this.OnInit(httpApplication);
        }

        /// <summary>
        /// Initializes any data/resources on HTTP module start.
        /// </summary>
        private void OnInit(HttpApplication httpApplication)
        {

        }

        /// <summary>
        /// Initializes any data/resources on application start.
        /// </summary>
        protected void OnStart(HttpApplication httpApplication)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(
                                                      "Nazardo.SP2013.WebAPI",
                                                      TraceSeverity.Medium,
                                                      EventSeverity.Information),
                                                  TraceSeverity.Medium,
                                                  string.Format("WebApiModule: Configuring Routes and VirtualPathProvider"),
                                                  null);

            var config = GlobalConfiguration.Configuration;
            config.Services.Replace(typeof(IAssembliesResolver), new WebApiAssemblyResolver());
            config.Services.Replace(typeof(IHttpControllerSelector), new Http.DefaultHttpControllerSelector(config));
            config.Services.Replace(typeof(IHttpControllerTypeResolver), new DefaultHttpControllerTypeResolver());

            GlobalConfiguration.Configure(RegisterApiConfig);

            HostingEnvironment.RegisterVirtualPathProvider(new WebAPIVirtualPathProvider());
        }

        public void Dispose()
        {
            // nothing to clean up.
        }

        private static void RegisterApiConfig(HttpConfiguration config)
        {
            // Web API routes
            config.Routes.Clear();
            RouteTable.Routes.Ignore("{resource}.axd/{*pathInfo}");
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCaseExceptDictionaryKeysResolver();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.EnsureInitialized();
        }
    }
}
