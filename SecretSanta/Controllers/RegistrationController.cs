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
        private readonly ISettingsRepository _settingsRepository;

        public RegistrationController(IUserRepository userRepository, ISettingsRepository settingsRepository)
        {
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(_settingsRepository.RegistrationOpen)
                return View(new SantaUserPostModel());
            return View("Message", Resources.Global.RegistrationClosed);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SantaUserPostModel model)
        {
            if(!_settingsRepository.RegistrationOpen)
                return View("Message", Resources.Global.RegistrationClosed);

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // save to db
            var domainModel = Mapper.Map<SantaUser>(model);
            _userRepository.InsertUser(domainModel);
            // TODO: change to confirmaton
            return View("Confirmation", model.Email);
        }
    }
}