namespace SecretSanta.Common.Interface
{
    public interface IConfigProvider
    {
        string ConnectionString { get; }
        string DataProtectionKey { get; }
        string HashSecret { get; }
        int SaltLength { get; }
    }
}