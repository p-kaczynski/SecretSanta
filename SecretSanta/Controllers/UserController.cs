using System;
using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Helpers;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles = SantaUserManager.AdminRole)]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IConfigProvider _configProvider;
        private readonly IEmailService _emailService;

        public UserController(IUserRepository userRepository, IConfigProvider configProvider, IEmailService emailService, IMapper mapper)
        {
            _userRepository = userRepository;
            _configProvider = configProvider;
            _emailService = emailService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return
            View(_userRepository.GetAllUsersWithoutProtectedData());
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View(new RegistrationPostModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(RegistrationPostModel model)
        {
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

            if (model.DateOfBirth.AddYears(_configProvider.MinimumAge) > DateTime.Today)
            {
                ModelState.AddModelError(nameof(RegistrationPostModel.DateOfBirth), Resources.Global.Registration_Form_DateOfBirth_NotEnough);
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
            var domainModel = _mapper.Map<SantaUser>(model);
            _userRepository.InsertUser(domainModel);

            _emailService.SendConfirmationEmail(domainModel);

            return View("Message", model: string.Format(Resources.Global.AdminUser_Created_Format, model.Email));
        }

        [HttpGet]
        public ActionResult RemoveUser(long id)
        {
            return View(_userRepository.GetUserWithoutProtectedData(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUser(SantaUser user)
        {
            if (_userRepository.WasAssigned())
                return View("Message", model: Resources.Global.Message_CannotRemoveAccountAfterAssignment);

            _userRepository.DeleteUser(user.Id);
            return RedirectToAction("Index");
        }
    }
}