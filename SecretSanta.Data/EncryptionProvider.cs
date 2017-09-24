using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
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
        private readonly int _saltLength;

        public EncryptionProvider(IConfigProvider configProvider)
        {
            // TODO: Protect against empty etc!
            _key = Encoding.UTF8.GetBytes(configProvider.DataProtectionKey);
            _saltLength = configProvider.SaltLength;
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

        private void WithCrypto<T>(T model, Action<T,PropertyInfo,ICryptoTransform> action) where T : ModelBase
        {
            var properties = _propertyCache.GetOrAdd(typeof(T), LoadPropertiesFromType);
            using (var aes = Aes.Create())
            {
                if(aes == null)
                    throw new Exception("Cannot initialize cryptographical provider. The application might be running on a system that does not provide appropriate cryptography. This is a fatal error.");

                aes.IV = model.IV;
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