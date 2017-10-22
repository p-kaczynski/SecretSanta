using System.Web.Mvc;

namespace SecretSanta.Controllers
{
    public class PrivacyController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}