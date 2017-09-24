using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IEncryptionProvider
    {
        void Decrypt<T>(T theModel) where T : ModelBase;
        void Encrypt<T>(T theModel) where T : ModelBase;
        byte[] NewSalt();
    }
}