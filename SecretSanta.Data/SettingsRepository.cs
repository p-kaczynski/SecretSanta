using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;
using Dapper;
using JetBrains.Annotations;
using SecretSanta.Common.Interface;

namespace SecretSanta.Data
{

    public class SettingsRepository : ISettingsRepository
    {
        private static class SettingKeys
        {
            public const string RegistrationOpen = "santa.registration_open";
        }

        private readonly string _connectionString;
        private readonly TimeSpan _cacheTime;

        public SettingsRepository(IConfigProvider configProvider)
        {
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

        private void SetSettingWithCache<T>(string key, T value, Func<T, string> factory)
        {
            SetSetting(key, factory(value));
            HttpRuntime.Cache.Remove(key);
            HttpRuntime.Cache.Add(key, value, null, DateTime.Now + _cacheTime, Cache.NoSlidingExpiration,
                CacheItemPriority.Default, null);
        }

        private T GetOrAddToCache<T>([NotNull] string key, Func<string, T> factory)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            return (T)(HttpRuntime.Cache.Get(key) ?? AddToCache(key, factory));
        }

        private T AddToCache<T>(string key, Func<string, T> factory)
        {
            var value = factory(key);
            HttpRuntime.Cache.Add(key, value, null,
                DateTime.Now + _cacheTime, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
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