using System;
using System.Web.Configuration;
using SecretSanta.Common.Interface;
using Utilities.Collections.NameValueCollection;

namespace SecretSanta
{
    public class ConfigProvider : IConfigProvider
    {
        private static class ConfigKeys
        {
            public const string ConnectionStringName = "SantaDB";
            public const string EncryptionKey = "santa.dataprotection.encryptionkey";
            public const string HashSecret = "santa.encryption.hashSecret";
            public const string SaltLength = "santa.user.salt_length";
            public const string PasswordLength = "santa.user.password_length";
            public const string DevMode = "santa.dev.devmode";
            public const string GenerateTestData = "santa.dev.generate_test_data";
            public const string SettingsCacheSeconds = "santa.cache.settings.seconds";
            public const string DefaultCountryTwoLetterCode = "santa.ui.default_country";
            public const string UICultureTwoLetterCode = "santa.ui.culture";
            public const string ResendConfirmationCooldown = "santa.user.resend_confirmation_cooldown_minutes";
            public const string PasswordResetCooldown = "santa.user.pasword_reset_cooldown_minutes";
            public const string PasswordResetValidFor = "santa.user.pasword_reset_valid_minutes";
            public const string SATSecret = "santa.encryption.sat.secret";
            public const string UseMailgun = "santa.email.use_mailgun";
            public const string MailgunDomain = "santa.email.mailgun_domain";
            public const string MailgunApiKey = "santa.email.mailgun_apikey";
            public const string MailgunFrom = "santa.email.mailgun_from";
            public const string AdminEmail = "santa.email.admin_email";
            public const string SendMessageNotifications = "santa.email.send_message_notifications";
            public const string AdultAge = "santa.user.adult_age";
            public const string MinimumAge = "santa.user.minimum_age";
        }

        public string ConnectionString 
            => WebConfigurationManager.ConnectionStrings[ConfigKeys.ConnectionStringName].ConnectionString;

        public string DataProtectionKey
            => WebConfigurationManager.AppSettings[ConfigKeys.EncryptionKey];
        public string HashSecret
            => WebConfigurationManager.AppSettings[ConfigKeys.HashSecret];

        public int SaltLength
            => WebConfigurationManager.AppSettings.GetInt(ConfigKeys.SaltLength, 16);

        public bool DevMode => WebConfigurationManager.AppSettings.GetBoolOrDefault(ConfigKeys.DevMode, false);

        public bool GenerateTestData =>
            WebConfigurationManager.AppSettings.GetBoolOrDefault(ConfigKeys.GenerateTestData, false);

        public TimeSpan SettingCacheTime =>
            TimeSpan.FromSeconds(WebConfigurationManager.AppSettings.GetInt(ConfigKeys.SettingsCacheSeconds, 15));

        public string DefaultCountryTwoLetterCode =>
            WebConfigurationManager.AppSettings[ConfigKeys.DefaultCountryTwoLetterCode]?.ToLower() ?? "en";

        public string UICultureTwoLetterCode => WebConfigurationManager.AppSettings[ConfigKeys.UICultureTwoLetterCode]?.ToLower() ?? "en";
        public TimeSpan ResendConfirmationCooldown =>
            TimeSpan.FromMinutes(WebConfigurationManager.AppSettings.GetInt(ConfigKeys.ResendConfirmationCooldown, 15));

        public TimeSpan PasswordResetCooldown 
            => TimeSpan.FromMinutes(WebConfigurationManager.AppSettings.GetInt(ConfigKeys.PasswordResetCooldown, 15));

        public TimeSpan PasswordResetValidFor 
            => TimeSpan.FromMinutes(WebConfigurationManager.AppSettings.GetInt(ConfigKeys.PasswordResetValidFor, 120));

        public string SATSecret
            => WebConfigurationManager.AppSettings[ConfigKeys.SATSecret];

        public int MinimumPasswordLength =>
            WebConfigurationManager.AppSettings.GetInt(ConfigKeys.PasswordLength, 10);

        public bool UseMailgun =>
            WebConfigurationManager.AppSettings.GetBoolOrDefault(ConfigKeys.UseMailgun, false);

        public string MailgunBaseDomain =>
            WebConfigurationManager.AppSettings[ConfigKeys.MailgunDomain];

        public string MailgunApiKey =>
            WebConfigurationManager.AppSettings[ConfigKeys.MailgunApiKey];

        public string MailgunFrom =>
            WebConfigurationManager.AppSettings[ConfigKeys.MailgunFrom];

        public string AdminEmail =>
            WebConfigurationManager.AppSettings[ConfigKeys.AdminEmail];

        public bool SendMessageEmails =>
            WebConfigurationManager.AppSettings.GetBoolOrDefault(ConfigKeys.SendMessageNotifications, true);

        public int AdultAge =>
            WebConfigurationManager.AppSettings.GetInt(ConfigKeys.AdultAge, 18);

        public int MinimumAge =>
            WebConfigurationManager.AppSettings.GetInt(ConfigKeys.MinimumAge, 16);
    }
}