using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;

namespace SecretSanta.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return
            View(_userRepository.GetAllUsersWithoutProtectedData());
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