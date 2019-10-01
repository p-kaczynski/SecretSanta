using System.Web.Mvc;
using NLog;
using SecretSanta.Filters;

namespace SecretSanta.Controllers
{
    [LogException]
    public abstract class BaseController : Controller
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected long? GetUserId() => ClaimHelper.GetUserId(User);
    }
}