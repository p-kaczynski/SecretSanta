using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using SecretSanta.Common.Interface;
using SecretSanta.Data;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Security;
using SecretSanta.Services;

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
            builder.RegisterSource(new ViewRegistrationSource());

            RegisterDependencies(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>().SingleInstance();
            builder.RegisterType<EncryptionProvider>().As<IEncryptionProvider>().SingleInstance();
            builder.RegisterType<CountryProvider>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<SecurityRepository>().As<ISantaUserStore>().SingleInstance();
            builder.RegisterType<SantaUserManager>().As<UserManager<SantaSecurityUser, string>>().As<ISantaAdminProvider>().SingleInstance();
            builder.RegisterType<EmailService>().As<IEmailService>().SingleInstance();
        }
    }
}