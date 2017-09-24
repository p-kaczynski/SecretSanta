using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Data;

namespace SecretSanta.DI
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            RegisterDependencies(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
            builder.RegisterType<EncryptionProvider>().As<IEncryptionProvider>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
        }
    }
}