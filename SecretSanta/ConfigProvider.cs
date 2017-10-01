﻿using System;
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
            public const string DevMode = "santa.dev.devmode";
            public const string SettingsCacheSeconds = "santa.cache.settings.seconds";
        }

        public string ConnectionString 
            => WebConfigurationManager.ConnectionStrings[ConfigKeys.ConnectionStringName].ConnectionString;

        public string DataProtectionKey
            => WebConfigurationManager.AppSettings[ConfigKeys.EncryptionKey];
        public string HashSecret
            => WebConfigurationManager.AppSettings[ConfigKeys.HashSecret];

        public int SaltLength
            => int.TryParse(WebConfigurationManager.AppSettings[ConfigKeys.SaltLength], out int length)
                ? length
                : 16;

        public bool DevMode => WebConfigurationManager.AppSettings.GetBoolOrDefault(ConfigKeys.DevMode, false);

        public TimeSpan SettingCacheTime =>
            TimeSpan.FromSeconds(WebConfigurationManager.AppSettings.GetInt(ConfigKeys.SettingsCacheSeconds, 15));
    }
}