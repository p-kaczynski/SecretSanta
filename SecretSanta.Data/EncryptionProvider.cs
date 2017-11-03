using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Scrypt;
using SecretSanta.Common.Helpers;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public sealed class EncryptionProvider : IEncryptionProvider, IDisposable
    {
        private readonly ConcurrentDictionary<Type,PropertyInfo[]> _propertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();
        private readonly byte[] _key;
        private readonly byte[] _secret;
        private readonly int _saltLength;

        public EncryptionProvider(IConfigProvider configProvider)
        {
            if(string.IsNullOrWhiteSpace(configProvider.DataProtectionKey))
                throw new ConfigurationErrorsException($"The config value for {nameof(configProvider.DataProtectionKey)} is null or whitespace. This is not allowed due to security concerns.");
            _key = SizeKey(Encoding.UTF8.GetBytes(configProvider.DataProtectionKey));

            if (string.IsNullOrWhiteSpace(configProvider.HashSecret))
                throw new ConfigurationErrorsException($"The config value for {nameof(configProvider.HashSecret)} is null or whitespace. This is not allowed due to security concerns.");
            _secret = Encoding.UTF8.GetBytes(configProvider.HashSecret);

            _saltLength = configProvider.SaltLength;
        }

        private static byte[] SizeKey(byte[] bytes)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new Exception(
                        "Cannot initialize cryptographical provider. The application might be running on a system that does not provide appropriate cryptography. This is a fatal error.");

                return FitBytes(bytes, aes.KeySize / 8);
            }
        }

        private static byte[] FitBytes(byte[] bytes, int size)
        {
            var validArray = new byte[size];
            if (bytes.Length == size)
                return bytes;

            // repeat or truncate
            for (var i = 0; i < size; ++i)
                validArray[i] = bytes[i % bytes.Length];

            return validArray;
        }

        public void Encrypt<T>(T theModel) where T : ModelBase
            => WithCrypto(theModel,
                (model, property, encryptor) => property.SetValue(model,
                    EncryptValue(encryptor, property.GetValue(model) as string)));

        public void Decrypt<T>(T theModel) where T : ModelBase
            => WithCrypto(theModel,
                (model, property, decryptor) => property.SetValue(model,
                    DecryptValue(decryptor, property.GetValue(model) as string)), decrypt: true);

        public byte[] CalculatePasswordHash(string password, byte[] associatedData = null)
        {
            var encoder = new ScryptEncoder();
            var hash = encoder.Encode(password);

            return Encoding.UTF8.GetBytes(hash);
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            var hashString = Encoding.UTF8.GetString(storedHash);

            var encoder = new ScryptEncoder();
            return encoder.Compare(password, hashString);
        }

        private void WithCrypto<T>(T model, Action<T,PropertyInfo,ICryptoTransform> action, bool decrypt = false) where T : ModelBase
        {
            var properties = _propertyCache.GetOrAdd(typeof(T), DataProtection.LoadDataProtectedPropertiesFromType);
            using (var aes = Aes.Create())
            {
                if(aes == null)
                    throw new Exception("Cannot initialize cryptographical provider. The application might be running on a system that does not provide appropriate cryptography. This is a fatal error.");

                aes.IV = FitBytes(model.IV(), aes.BlockSize / 8);
                aes.Key = _key;
                aes.Padding = PaddingMode.PKCS7;



                foreach (var property in properties)
                {
                    using (var transform = decrypt ? aes.CreateDecryptor(aes.Key, aes.IV) : aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        action(model, property, transform);
                    }
                }
                aes.Clear();
            }
        }

        private static string EncryptValue([NotNull] ICryptoTransform encryptor, [CanBeNull] string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                    sw.Write(value);

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static string DecryptValue([NotNull] ICryptoTransform decryptor, [CanBeNull] string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using (var ms = new MemoryStream(Convert.FromBase64String(value)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
                return sr.ReadToEnd();
        }

        public void Dispose()
        {
            _rngProvider?.Dispose();
        }
    }
}