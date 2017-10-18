using System;
using System.Reflection;
using System.Web.Mvc;

namespace SecretSanta.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HelpTextAttribute : Attribute, IMetadataAware
    {
        public const string HelpTextPropertyName = "custom.help_text";
        private readonly string _helpText;
        public HelpTextAttribute(string helpText)
        {
            _helpText = helpText;
        }

        public HelpTextAttribute(Type resourceType, string propertyName)
        {
            var property = resourceType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            if (property != null && property.GetMethod != null)
                _helpText = property.GetValue(null) as string;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues[HelpTextPropertyName] = _helpText;
        }
    }
}