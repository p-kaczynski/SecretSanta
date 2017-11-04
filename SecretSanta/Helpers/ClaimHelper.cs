using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SecretSanta
{
    public static class ClaimHelper
    {
        public static string GetUserName(IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal identity)) return null;
            return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        }
    }
}