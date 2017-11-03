using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Common.Result;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles = SantaUserManager.AdminRole)]
    public class AssignmentController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public AssignmentController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (_userRepository.WasAssigned())
            {
                return View(_userRepository.GetAssignments());
            }
            return View((AssignmentResult)null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign()
        {
            if (_userRepository.WasAssigned())
                return RedirectToAction("Index");

            var assignments = _userRepository.AssignRecipients();

            return View("AssignmentCompleted", assignments);
        }
    }
}