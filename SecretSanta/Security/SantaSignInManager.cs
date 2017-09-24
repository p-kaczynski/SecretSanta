using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Security
{
    public class SantaSignInManager : SignInManager<SantaSecurityUser, string>
    {
        public SantaSignInManager(UserManager<SantaSecurityUser, string> userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }
    }
}