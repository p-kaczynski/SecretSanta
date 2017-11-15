using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace SecretSanta.Helpers
{
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            DateTime dateTime;

            var isDate = DateTime.TryParse(value.AttemptedValue, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out dateTime);

            if (!isDate)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, Resources.Global.Registration_Form_DateOfBirth_Invalid);
                return DateTime.UtcNow;
            }

            return dateTime;
        }
    }
}