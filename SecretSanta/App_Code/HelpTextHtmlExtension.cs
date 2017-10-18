using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SecretSanta.Models.Attributes;

namespace SecretSanta
{
    public static class HelpTextHtmlExtension
    {
        public static IHtmlString HelpTextFor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> ex
        )
        {
            var metadata = ModelMetadata.FromLambdaExpression(ex, html.ViewData);
            var helpTextBuilder = new StringBuilder();
            if (metadata.ModelType.IsValueType ? metadata.ModelType.GetCustomAttributes(typeof(RequiredAttribute), false).Any() : metadata.IsRequired)
            {
                helpTextBuilder.Append('*');
            }
            if (metadata.AdditionalValues.ContainsKey(HelpTextAttribute.HelpTextPropertyName))
            {
                helpTextBuilder.Append(metadata.AdditionalValues[HelpTextAttribute.HelpTextPropertyName] as string);
            }
            if (helpTextBuilder.Length == 0)
                return MvcHtmlString.Empty;

            var helpTextHtml = new TagBuilder("p");
            helpTextHtml.Attributes.Add("class", "form-text text-muted");
            helpTextHtml.InnerHtml = helpTextBuilder.ToString();

            return new MvcHtmlString(helpTextHtml.ToString());

        }
    }
}