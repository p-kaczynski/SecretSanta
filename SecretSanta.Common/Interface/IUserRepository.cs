using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IUserRepository
    {
        SantaUser GetUser(long id);
        long InsertUser(SantaUser user);
    }
}