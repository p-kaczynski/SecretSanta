using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionProvider _encryptionProvider;

        public RegistrationController(IUserRepository userRepository, IEncryptionProvider encryptionProvider)
        {
            _userRepository = userRepository;
            _encryptionProvider = encryptionProvider;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new SantaUserPostModel());
        }

        [HttpPost]
        public ActionResult Index(SantaUserPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // save to db

            // TODO: change to confirmaton
            return View(model);
        }
    }
}