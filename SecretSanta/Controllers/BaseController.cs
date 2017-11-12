using System.Web.Mvc;
using NLog;

namespace SecretSanta.Controllers
{
    public abstract class BaseController : Controller
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected long? GetUserId() => ClaimHelper.GetUserId(User);
    }
}