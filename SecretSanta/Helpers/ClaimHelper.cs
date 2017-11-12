using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Security;

namespace SecretSanta
{
    public static class ClaimHelper
    {
        public static string GetUserName(IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal identity)) return null;
            return identity.GetClaimValue(ClaimTypes.GivenName);
        }

        public static long? GetUserId(IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal identity)) return null;
            var id = identity.GetClaimValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(id))
                return null;
            var userId = SantaSecurityUser.GetId(id, out var isAdmin);
            return isAdmin ? null : (long?)userId;
        }

        private static string GetClaimValue(this ClaimsPrincipal principal, string claimType) =>
            principal.Claims.SingleOrDefault(c => c.Type == claimType)?.Value;
    }
}