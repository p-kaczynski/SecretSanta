using System.Web.Mvc;
using JetBrains.Annotations;
using NLog;

namespace SecretSanta.Filters
{
    public class LogExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException([NotNull] ExceptionContext filterContext) 
            => LogManager.GetCurrentClassLogger(filterContext.Controller.GetType()).Error(filterContext.Exception);
    }
}