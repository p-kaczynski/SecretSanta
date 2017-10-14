using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SecretSanta.DI;

namespace SecretSanta
{
    public class MvcApplication : System.Web.HttpApplication
    {
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
        }
    }
}
