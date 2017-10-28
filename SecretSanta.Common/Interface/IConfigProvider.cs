using System;

namespace SecretSanta.Common.Interface
{
    public interface IConfigProvider
    {
        string ConnectionString { get; }
        string DataProtectionKey { get; }
        string HashSecret { get; }
        int SaltLength { get; }
        bool DevMode { get; }
        bool GenerateTestData { get; }
        TimeSpan SettingCacheTime { get; }
        string DefaultCountryTwoLetterCode { get; }
        string UICultureTwoLetterCode { get; }
        TimeSpan ResendConfirmationCooldown { get; }
        TimeSpan PasswordResetCooldown { get; }
        TimeSpan PasswordResetValidFor { get; }
        string SATSecret { get; }
        int MinimumPasswordLength { get; }

        bool UseMailgun { get; }

        string MailgunBaseDomain { get; }
        string MailgunApiKey { get; }
        string MailgunFrom { get; }
    }
}