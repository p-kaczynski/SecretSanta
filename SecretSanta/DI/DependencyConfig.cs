using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using SaturnV;
using SecretSanta.Common;
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
            builder.RegisterType<TriStateAssignmentAlgorithm>().As<IAssignmentAlgorithm>().SingleInstance();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>().SingleInstance();
            builder.RegisterType<EncryptionProvider>().As<IEncryptionProvider>().SingleInstance();
            builder.RegisterType<CountryProvider>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<SecurityRepository>().As<ISantaUserStore>().SingleInstance();
            builder.RegisterType<SantaUserManager>().As<UserManager<SantaSecurityUser, string>>().As<ISantaAdminProvider>().SingleInstance();
            builder.RegisterType<EmailService>().As<IEmailService>().SingleInstance();
            builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance();
            builder.RegisterType<MessageRepository>().As<IMessageReadOnlyRepository>().As<IMessageRepository>().SingleInstance();
            builder.Register(context =>
            {
                var config = context.Resolve<IConfigProvider>();
                return new YourPasswordSucks.PasswordValidator(
                    new YourPasswordSucks.PasswordValidatorSettings
                    {
                        // rest leave with OWASP defaults
                        MinimumPasswordLength = config.MinimumPasswordLength
                    });
            });

            // expiry, as needs to expire as password reset links are dangerous
            builder.Register(context =>
                {
                    var config = context.Resolve<IConfigProvider>();
                    return new SecureAccessTokenSource(new SecureAccessTokenSettings
                    {
                        Secret = config.SATSecret,
                        EnsureAtLeastValidFor = true,
                        ValidateData = true,
                        ValidateTime = true,
                        ValidFor = config.PasswordResetValidFor
                    });
                }).Keyed<SecureAccessTokenSource>(TokenSourceType.PasswordReset).SingleInstance();

            // no expiry, just provide a token that can be validated
            builder.Register(context =>
                {
                    var config = context.Resolve<IConfigProvider>();
                    return new SecureAccessTokenSource(new SecureAccessTokenSettings
                    {
                        Secret = config.SATSecret,
                        ValidateData = true,
                    });
                }).Keyed<SecureAccessTokenSource>(TokenSourceType.EmailConfirmation).SingleInstance();
        }
    }
}