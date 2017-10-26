using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;
using SecretSanta.DI;

namespace SecretSanta
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependencyConfig.RegisterDependencies();
            // Must be after DI:
            MappingConfig.Configure();
            BindingConfig.Configure();
            Log.Info("App is starting.");
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            Log.Error(exception, $"Unhandled exception was detected by {nameof(Application_Error)}");
        }


    }
}
