using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Autofac.Features.Indexed;
using AutoMapper;
using Microsoft.AspNet.Identity;
using SaturnV;
using SecretSanta.Common;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.Models.Extensions;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles=SantaUserManager.UserRole)]
    public class AccountController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<SantaSecurityUser, string> _userManager;
        private readonly IEmailService _emailService;
        private readonly SecureAccessTokenSource _emailConfirmationTokenSource;
        private readonly SecureAccessTokenSource _passwordResetTokenSource;

        public AccountController(IUserRepository userRepository, UserManager<SantaSecurityUser, string> userManager, IEmailService emailService, IIndex<TokenSourceType,SecureAccessTokenSource> satIndex)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _emailService = emailService;
            _emailConfirmationTokenSource = satIndex[TokenSourceType.EmailConfirmation];
            _passwordResetTokenSource = satIndex[TokenSourceType.PasswordReset];
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return RedirectToAction("Index", "Home");

            var santaUser = _userRepository.GetUser(userId);
            if (santaUser == null)
            {
                // TODO: Notify admin
                Log.Error($"Cannot load a user id={userId}");
                return HttpNotFound();
            }
            var model = Mapper.Map<SantaUserViewModel>(santaUser);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> RemoveAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return RedirectToAction("Index", "Home");

            var santaUser = _userRepository.GetUser(userId);
           
            return View(santaUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveAccount(SantaUser model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            if (_userRepository.WasAssigned())
                return View("Message", model: Resources.Global.Message_CannotRemoveAccountAfterAssignment);

            // TODO: More info
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return RedirectToAction("Index", "Home");

            var santaUser = _userRepository.GetUser(userId);

            if (model.Id != santaUser.Id)
                return RedirectToAction("Index");

            // sign out
            HttpContext.GetOwinContext().Authentication
                .SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            _userRepository.DeleteUser(userId);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm(long userId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return HttpNotFound();

            var user = _userRepository.GetUserWithoutProtectedData(userId);
            if (user == null)
                return View("Message", model: Resources.Global.EmailConfirmation_TokenInvalid);

            if (!_emailConfirmationTokenSource.Validate(user.GetEmailConfirmationTokenGenerationInputString(), token))
                return View("Message", model:Resources.Global.EmailConfirmation_TokenInvalid);

            _userRepository.EmailConfirm(userId);
            return View("Message", model:Resources.Global.EmailConfirmedMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmation()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return RedirectToAction("Index", "Home");

            var domainModel = _userRepository.GetUserWithoutProtectedData(userId);
            if (domainModel == null)
            {
                // TODO: This is bad
                Log.Error($"Tried to retrieve current user id={userId} from repository, but got null");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            _emailService.SendConfirmationEmail(domainModel);

            return View("Message", model:Resources.Global.Message_ResendConfirmation);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordPostModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordPostModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userRepository.GetUserWithoutProtectedDataByEmail(model.EmailAddress);
            if (user == null)
                return View("Message", model: Resources.Global.PasswordReset_Sent);

            // we have user, so email is ok, send a reset
            if(_emailService.SendPasswordResetEmail(user))
                return View("Message", model:Resources.Global.PasswordReset_Sent);

            // error!
            return View("Message", model: Resources.Global.Email_SendingFailure);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(long userId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return HttpNotFound();

            var user = _userRepository.GetUserWithoutProtectedData(userId);
            if (user == null)
                return View("Message", model: Resources.Global.PasswordReset_TokenInvalid);

            return View(new PasswordResetViewModel { Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(PasswordResetViewModel model)
        {
            if (model.ConfirmNewPassword != model.NewPassword)
            {
                ModelState.AddModelError(nameof(PasswordResetViewModel.ConfirmNewPassword),Resources.Global.PasswordConfirmation_NoMatch);
                model.NewPassword = null;
                model.ConfirmNewPassword = null;
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                model.NewPassword = null;
                model.ConfirmNewPassword = null;
                return View(model);
            }

            var user = _userRepository.GetUserWithoutProtectedData(model.UserId);
            if (user == null)
                return View("Message", model: Resources.Global.PasswordReset_TokenInvalid);

            if (!_passwordResetTokenSource.Validate(user.GetPasswordResetTokenGenerationInputString(), model.Token))
            {
                ModelState.AddModelError(nameof(PasswordResetViewModel.Token), Resources.Global.PasswordReset_TokenInvalid);
                return View(model);
            }

            // all good, reset password
            _userRepository.SetPassword(Mapper.Map<PasswordResetModel>(model));
            return View("Message", model: Resources.Global.PasswordReset_Success);
        }

        [HttpGet]
        public async Task<ActionResult> EditAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return RedirectToAction("Index", "Home");

            var santaUser = _userRepository.GetUser(userId);
            SantaUserPostModel model = Mapper.Map<SantaUserViewModel>(santaUser);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccount(SantaUserPostModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var updateModel = Mapper.Map<SantaUser>(model);
            updateModel.Id = userId;

            var updateResult = _userRepository.UpdateUser(updateModel);
            if (!updateResult.Success)
            {
                if (updateResult.EmailUnavailable)
                {
                    ModelState.AddModelError(nameof(SantaUserPostModel.Email), Resources.Global.EmailTaken);
                    return View(model);
                }

                return View("Message", model: Resources.Global.Message_Error);
            }

            if (updateResult.EmailChanged)
            {
                _emailService.SendConfirmationEmail(updateModel);

                // sign out, email is used to find people in the user db!
                HttpContext.GetOwinContext().Authentication
                    .SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                return View("Message", model: string.Format(Resources.Global.User_Edit_EmailSent, updateModel.Email));
            }

            return RedirectToAction("Index");
        }
    }
}