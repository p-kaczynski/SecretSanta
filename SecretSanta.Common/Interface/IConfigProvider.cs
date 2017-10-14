﻿using System;

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
        TimeSpan ResendConfirmationCooldown { get;}
    }
}