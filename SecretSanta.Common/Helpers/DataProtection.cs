using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SecretSanta.Domain.Attributes;

namespace SecretSanta.Common.Helpers
{
    public static class DataProtection
    {
        public static void ClearDataProtected([CanBeNull] this object obj)
        {
            if (obj == null)
                return;

            foreach(var dataProtectedProperty in LoadDataProtectedPropertiesFromType(obj.GetType()))
                dataProtectedProperty.SetValue(obj, null);
        }

        public static PropertyInfo[] LoadDataProtectedPropertiesFromType(Type type) => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.CanRead && prop.CanWrite && prop.PropertyType == typeof(string) &&
                           prop.GetCustomAttribute<DataProtectionAttribute>() != null).ToArray();
    }
}