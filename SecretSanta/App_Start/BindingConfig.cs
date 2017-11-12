using System;
using System.Web.Mvc;
using SecretSanta.Helpers;
using SecretSanta.Models;

namespace SecretSanta
{
    public static class BindingConfig
    {
        public static void Configure()
        {
            ModelBinders.Binders.Add(typeof(CountryEntryViewModel), new CountryEntryViewModelModelBinder());
        }

        class CountryEntryViewModelModelBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueResult == null)
                {
                    return null;
                }

                var modelState = new ModelState
                {
                    Value = valueResult
                };
                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

                // Try to parse the value.
                if (!int.TryParse(valueResult.AttemptedValue, out var parsedValue))
                {
                    // If you can't parse it, add a FormatException to the error list.
                    modelState.Errors.Add(new FormatException());
                }

                // On success, return the parsed value; on fail, return null.
                return new CountryEntryViewModel{Id = parsedValue};
            }
        }
        
    }
}