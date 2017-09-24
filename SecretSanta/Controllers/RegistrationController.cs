using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
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
            var domainModel = Mapper.Map<SantaUser>(model);
            _userRepository.InsertUser(domainModel);
            // TODO: change to confirmaton
            return RedirectToAction(nameof(Confirmation), new {domainModel});
        }

        public ActionResult Confirmation(SantaUser model)
        {
            if (model == null)
                return HttpNotFound();

            return View(model);
        }
    }
}