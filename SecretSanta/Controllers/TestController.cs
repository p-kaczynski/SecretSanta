using System.Text;
using System.Web.Mvc;
using SecretSanta.Common.Interface;

namespace SecretSanta.Controllers
{
    public class TestController : Controller
    {
        private readonly IEncryptionProvider _encryptionProvider;

        public TestController(IEncryptionProvider encryptionProvider)
        {
            _encryptionProvider = encryptionProvider;
        }

        // GET
        [HttpGet]
        public ActionResult Index()
        {
            return
            View();
        }

        [HttpPost]
        public ActionResult Index(string password)
        {
            ViewBag.Password = password;

            var hash = _encryptionProvider.CalculatePasswordHash(password);
            ViewBag.Hash = Encoding.UTF8.GetString(hash);

            var validates = _encryptionProvider.VerifyPasswordHash(password, hash);
            var notValidates = _encryptionProvider.VerifyPasswordHash("LubiePlacki", hash);

            ViewBag.Validates = validates.ToString();
            ViewBag.Other = notValidates.ToString();

            return View();
        }
    }
}