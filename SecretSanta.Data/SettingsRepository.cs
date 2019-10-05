using System;
using System.Data.SqlClient;
using Dapper;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using SecretSanta.Common.Interface;

namespace SecretSanta.Data
{

    public class SettingsRepository : ISettingsRepository
    {
        private readonly IMemoryCache _cache;

        private static class SettingKeys
        {
            public const string RegistrationOpen = "santa.registration_open";
        }

        private readonly string _connectionString;
        private readonly TimeSpan _cacheTime;

        public SettingsRepository([NotNull] IConfigProvider configProvider, [NotNull] IMemoryCache cache)
        {
            _cache = cache;
            _connectionString = configProvider.ConnectionString;
            _cacheTime = configProvider.SettingCacheTime;
        }

        private const string UpsertQuery =
            "IF EXISTS( SELECT * FROM SantaSettings WHERE [Key] = @key )\r\n" +
            "BEGIN\r\n" +
            "UPDATE SantaSettings SET [Value] = @value WHERE [Key] = @key\r\n" +
            "END\r\n" +
            "ELSE\r\n" +
            "INSERT INTO SantaSettings([Key],[Value]) VALUES (@key,@value)";

        private string GetSetting([NotNull] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.QuerySingleOrDefault<string>("SELECT Value FROM SantaSettings WHERE [Key] = @key", new {key});
            }
        }

        private void SetSetting([NotNull] string key, [CanBeNull] string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(UpsertQuery, new {key, value});
            }
        }

        private void SetSettingWithCache<T>([NotNull] string key, T value, [NotNull] Func<T, string> factory)
        {
            SetSetting(key, factory(value));
            _cache.Set(key, value, _cacheTime);
        }

        private T GetOrAddToCache<T>([NotNull] string key, [NotNull] Func<string, T> factory)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            return _cache.GetOrCreate(key,e=>
            {
                e.AbsoluteExpirationRelativeToNow = _cacheTime;
                return AddToCache(key, factory);
            });
        }

        private T AddToCache<T>(string key, [NotNull] Func<string, T> factory)
        {
            var value = factory(key);
            _cache.Set(key, value, _cacheTime);
            return value;
        }

        public bool RegistrationOpen
        {
            get => GetOrAddToCache(SettingKeys.RegistrationOpen,
                key => bool.TryParse(GetSetting(key), out var result) && result);
            set => SetSettingWithCache(SettingKeys.RegistrationOpen, value, boolean => boolean.ToString());
        }


    }
}