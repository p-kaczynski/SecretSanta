using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.SecurityModels;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<SantaSecurityUser, string> _userManager;

        public HomeController(IUserRepository userRepository, UserManager<SantaSecurityUser, string> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }


        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start");

            if (User.IsInRole(SantaUserManager.AdminRole))
                return RedirectToAction("Index", "Verification");

            if (User.IsInRole(SantaUserManager.UserRole))
                return RedirectToAction("Home");

            // Should not happen, we only have two roles
            return HttpNotFound();
        }

        [Authorize(Roles=SantaUserManager.UserRole)]
        public async Task<ActionResult> Home()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return HttpNotFound();

            var userId = SantaSecurityUser.GetId(user.Id, out var isAdmin);
            if (isAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            var santaUser = _userRepository.GetUserWithoutProtectedData(userId);
            if (santaUser == null)
                return HttpNotFound();

            var model = Mapper.Map<UserHomeViewModel>(santaUser);

            var assignedUserId = _userRepository.GetAssignedPartnerIdForUser(userId);
            if (assignedUserId.HasValue)
            {
                model.AssignmentPerformed = true;
                // Check assignment
                var outbound = _userRepository.GetOutboundAssignment(userId);
                var inbound = _userRepository.GetInboundAssignment(userId);

                // Has gift been sent?
                model.InboundGiftEnRoute = inbound.Sent;

                // Has gift been confirmed as received?
                model.InboundGiftArrived = inbound.Received;

                // Has outbound gift been received?
                model.OutboundGiftArrived = outbound.Received;

                // Has outbound gift been sent?
                model.OutboundGiftEnRoute = outbound.Sent;
                if(!outbound.Sent)
                {
                    // nope, show that it awaits
                    var assignedUser = _userRepository.GetUser(assignedUserId.Value);
                    if (assignedUser == null)
                    {
                        // TODO: NOTIFY ADMIN - THIS IS BAD
                        Log.Error($"Tried to retrieve current user id={userId} from repository, but got null");
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }
                    model.Assignment = Mapper.Map<AssignmentViewModel>(assignedUser);
                }
            }

            return View(model);
        }

        public ActionResult Start()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }
    }
}