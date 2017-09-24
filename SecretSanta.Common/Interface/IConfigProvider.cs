namespace SecretSanta.Common.Interface
{
    public interface IConfigProvider
    {
        string ConnectionString { get; }
        string DataProtectionKey { get; }
        int SaltLength { get; }
    }
}