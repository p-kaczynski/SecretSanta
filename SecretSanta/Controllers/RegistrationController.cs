using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IEmailService _emailService;

        public RegistrationController(IUserRepository userRepository, ISettingsRepository settingsRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(_settingsRepository.RegistrationOpen)
                return View(new SantaUserPostModel());
            return View("Message", model:Resources.Global.RegistrationClosed);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SantaUserPostModel model)
        {
            if(!_settingsRepository.RegistrationOpen)
                return View("Message", model:Resources.Global.RegistrationClosed);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // check fo existing user
            if (!_userRepository.CheckEmail(model.Email))
            {
                ModelState.AddModelError(nameof(SantaUser.Email), Resources.Global.EmailTaken);
                return View(model);
            }

            // save to db
            var domainModel = Mapper.Map<SantaUser>(model);
            _userRepository.InsertUser(domainModel);
            
            _emailService.SendConfirmationEmail(domainModel);

            return View("Confirmation", (object)model.Email);
        }
    }
}