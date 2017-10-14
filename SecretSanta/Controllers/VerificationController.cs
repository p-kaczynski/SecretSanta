using System.Linq;
using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles = SantaUserManager.AdminRole)]
    public class VerificationController : Controller
    {
        private readonly IUserRepository _userRepository;

        public VerificationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_userRepository.GetAllUsersWithoutProtectedData().Where(u=>!u.AdminConfirmed).ToArray());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminConfirmUser(ConfirmationPostModel model)
        {
            ConfirmationHelperModel result;
            try
            {
                _userRepository.AdminConfirm(model.Id);
                result = ConfirmationHelperModel.ConfirmedModel;
            }
            catch
            {
                // TODO: Log
                result = ConfirmationHelperModel.UnconfirmedModel(model.Id);
            }
            return PartialView("_ConfirmationCell", result);
        }

        public ActionResult GetAwaitingVerifications()
        {
            return PartialView("_BadgeCounter",
                _userRepository.GetAllUsersWithoutProtectedData().Count(u => !u.AdminConfirmed));
        }
    }
}