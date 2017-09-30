using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Common.Interface
{
    public interface ISantaUserStore : IUserStore<SantaSecurityUser>
    {
        IList<SantaAdmin> GetAllAdmins();
    }
}