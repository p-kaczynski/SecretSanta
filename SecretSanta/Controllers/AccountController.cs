using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles=SantaUserManager.UserRole)]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<SantaSecurityUser, string> _userManager;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IEmailService _emailService;

        public AccountController(IUserRepository userRepository, UserManager<SantaSecurityUser, string> userManager, IEncryptionProvider encryptionProvider, IEmailService emailService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _encryptionProvider = encryptionProvider;
            _emailService = emailService;
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
            return View(santaUser);
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
            // TODO: change to a viewmodel...
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
                return View("Message", model: Resources.Global.TokenInvalid);

            if (!_encryptionProvider.VerifyEmailVerificationToken(user, token))
                return View("Message", model:Resources.Global.TokenInvalid);

            _userRepository.EmailConfirm(userId);
            return View("Message", model:Resources.Global.EmailConfirmedMessage);
        }

        [HttpGet]
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
                // TODO: Notify admin, this is bad
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            _emailService.SendConfirmationEmail(domainModel);

            return View("Message", model:Resources.Global.Message_ResendConfirmation);
        }
    }
}