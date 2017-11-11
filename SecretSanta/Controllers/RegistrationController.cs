using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Helpers;
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
                return View(new RegistrationPostModel());
            return View("Message", model:Resources.Global.RegistrationClosed);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegistrationPostModel model)
        {
            if(!_settingsRepository.RegistrationOpen)
                return View("Message", model:Resources.Global.RegistrationClosed);

            if (!ModelState.IsValid)
            {
                model.Password = null;
                model.RepeatPassword = null;
                return View(model);
            }

            // check fo existing user
            if (!_userRepository.CheckEmail(model.Email))
            {
                ModelState.AddModelError(nameof(RegistrationPostModel.Email), Resources.Global.EmailTaken);
                model.Password = null;
                model.RepeatPassword = null;
                return View(model);
            }

            if (model.Password != model.RepeatPassword)
            {
                ModelState.AddModelError(nameof(RegistrationPostModel.RepeatPassword), Resources.Global.Registration_Form_Repeat_Password_Invalid);
                model.Password = null;
                model.RepeatPassword = null;
                return View(model);
            }

            // set the correct fb uri:
            model.FacebookProfileUrl = FacebookUriHelper.GetUniformFacebookUri(model.FacebookProfileUrl);

            if (model.FacebookProfileUrl == null || !_userRepository.CheckFacebookProfileUri(model.FacebookProfileUrl))
            {
                ModelState.AddModelError(nameof(RegistrationPostModel.FacebookProfileUrl), Resources.Global.FacebookURL_Invalid);
                model.Password = null;
                model.RepeatPassword = null;
                return View(model);
            }

            // save to db
            var domainModel = Mapper.Map<SantaUser>(model);
            _userRepository.InsertUser(domainModel);
            
            _emailService.SendConfirmationEmail(domainModel);

            return View("Confirmation", model: model.Email);
        }
    }
}