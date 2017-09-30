using System.Collections;
using System.Collections.Generic;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Common.Interface
{
    public interface ISantaAdminProvider
    {
        IList<SantaAdmin> GetAllAdmins();
    }
}