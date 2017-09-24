using System.Web.Configuration;
using SecretSanta.Common.Interface;

namespace SecretSanta
{
    public class ConfigProvider : IConfigProvider
    {
        private static class ConfigKeys
        {
            public const string ConnectionStringName = "SantaDB";
        }

        public string ConnectionString 
            => WebConfigurationManager.ConnectionStrings[ConfigKeys.ConnectionStringName].ConnectionString;

        public string DataProtectionKey
            => WebConfigurationManager.AppSettings["santa.dataprotection.encryptionkey"];

        public int SaltLength
            => int.TryParse(WebConfigurationManager.AppSettings["santa.user.salt_length"], out int length)
                ? length
                : 10;
    }
}