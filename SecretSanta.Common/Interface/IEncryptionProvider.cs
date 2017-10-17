using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IEncryptionProvider
    {
        void Decrypt<T>(T theModel) where T : ModelBase;
        void Encrypt<T>(T theModel) where T : ModelBase;
        byte[] CalculatePasswordHash(string password, byte[] associatedData = null);
        bool VerifyPasswordHash(string password, byte[] storedHash);
    }
}