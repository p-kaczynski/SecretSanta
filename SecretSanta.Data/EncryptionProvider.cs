using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Attributes;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public class EncryptionProvider : IEncryptionProvider, IDisposable
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
                (model, property, encryptor) => property.SetValue(model,
                    DecryptValue(encryptor, property.GetValue(model) as string)));

        public byte[] NewSalt()
        {
            var result = new byte[_saltLength];
            _rngProvider.GetBytes(result);
            return result;
        }

        public byte[] CalculatePasswordHash(string password, byte[] associatedData = null)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var salt = new byte[16];
            _rngProvider.GetBytes(salt);
            var config = new Argon2Config
            {
                Type = Argon2Type.DataDependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 10,
                MemoryCost = 65536,
                Lanes = 5,
                Threads = Environment.ProcessorCount,
                Password = passwordBytes,
                Salt = salt, // >= 8 bytes if not null
                Secret = _secret, // from config
                AssociatedData = associatedData, // from item
                HashLength = 32 // >= 4
            };
            var argon2A = new Argon2(config);
            using (var hashA = argon2A.Hash())
            {
                return Encoding.UTF8.GetBytes(config.EncodeString(hashA.Buffer));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var argonString = Encoding.UTF8.GetString(storedHash);
            var configOfPasswordToVerify = new Argon2Config
            {
                Password = passwordBytes,
                Secret = _secret,
                Threads = 1
            };
            SecureArray<byte> hashB = null;
            try
            {
                if (configOfPasswordToVerify.DecodeString(argonString, out hashB) && hashB != null)
                {
                    var argon2ToVerify = new Argon2(configOfPasswordToVerify);
                    using (var hashToVerify = argon2ToVerify.Hash())
                        return !hashB.Buffer.Where((b, i) => b != hashToVerify[i]).Any();
                }
            }
            finally
            {
                hashB?.Dispose();
            }
            return false;
        }

        private void WithCrypto<T>(T model, Action<T,PropertyInfo,ICryptoTransform> action) where T : ModelBase
        {
            var properties = _propertyCache.GetOrAdd(typeof(T), LoadPropertiesFromType);
            using (var aes = Aes.Create())
            {
                if(aes == null)
                    throw new Exception("Cannot initialize cryptographical provider. The application might be running on a system that does not provide appropriate cryptography. This is a fatal error.");

                aes.IV = FitBytes(model.IV(), aes.BlockSize / 8);
                aes.Key = _key;

                var encryptor = aes.CreateEncryptor(aes.Key
                    , aes.IV);

                foreach (var property in properties)
                    action(model, property, encryptor);
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

        private static string DecryptValue([NotNull] ICryptoTransform encryptor, [CanBeNull] string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using (var ms = new MemoryStream(Convert.FromBase64String(value)))
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
                return sr.ReadToEnd();
        }

        private static PropertyInfo[] LoadPropertiesFromType(Type type) => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.CanRead && prop.CanWrite && prop.PropertyType == typeof(string) &&
                           prop.GetCustomAttribute<DataProtectionAttribute>() != null).ToArray();

        public void Dispose()
        {
            _rngProvider?.Dispose();
        }
    }
}