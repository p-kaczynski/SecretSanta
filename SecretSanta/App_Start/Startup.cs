using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Security;

namespace SecretSanta
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = (DependencyResolver.Current as AutofacDependencyResolver)?.ApplicationContainer;
            if (container == null)
                throw new ApplicationException($"DependencyResolver.Current is null in {nameof(Startup)}.{nameof(Configuration)}");

            app.UseAutofacMiddleware(container);
            app.CreatePerOwinContext<SantaSignInManager>((options, context) => new SantaSignInManager(context.GetAutofacLifetimeScope().Resolve<UserManager<SantaSecurityUser,string>>(), context.Authentication));

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login"),
            });
        }
    }
}