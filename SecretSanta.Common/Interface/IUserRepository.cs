using System.Collections.Generic;
using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IUserRepository
    {
        SantaUser GetUser(long id);
        long InsertUser(SantaUser user);

        SantaUser GetUserWithoutProtectedData(long id);
        IList<SantaUser> GetAllUsersWithoutProtectedData();

        void AdminConfirm(long id);
    }
}