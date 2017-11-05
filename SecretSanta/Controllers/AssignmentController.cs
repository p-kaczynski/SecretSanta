using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using SecretSanta.Common.Interface;
using SecretSanta.Common.Result;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize(Roles = SantaUserManager.AdminRole)]
    public class AssignmentController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public AssignmentController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendAssignmentEmails()
        {
            if (!_userRepository.WasAssigned())
                return RedirectToAction("Index");

            var assignments = _userRepository.GetAssignments();
            var assignmentModels = new ConcurrentBag<AssignmentEmailViewModel>();
            var abandonedModels = new ConcurrentBag<AbandonedEmailViewModel>();
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 5
            };
            Parallel.ForEach(assignments.Assignments,parallelOptions,
                assignment =>
                {
                    var sender = assignments.UserDisplayById[assignment.GiverId];
                    var target = assignments.UserDisplayById[assignment.RecepientId];

                    var success = _emailService.SendAssignmentEmail(sender, target);
                    
                        assignmentModels.Add(new AssignmentEmailViewModel
                        {
                            SenderDisplayName = sender.DisplayName,
                            SenderEmail = sender.Email,
                            TargetDisplayName = target.DisplayName,
                            TargetEmail = target.Email,
                            Success = success
                        });
                    
                });
            Parallel.ForEach(assignments.Abandoned, parallelOptions,
                abandoned =>
                {
                    var user = assignments.UserDisplayById[abandoned.SantaUserId];
                    var success = _emailService.SendAbandonmentEmail(user,
                        abandoned.Reason);
                    abandonedModels.Add(new AbandonedEmailViewModel
                    {
                        DisplayName = user.DisplayName,
                        Email = user.Email,
                        Reason = abandoned.Reason,
                        Success = success
                    });
                    
                });

            var model = new SendAssignmentEmailsViewModel
            {
                Assignments = assignmentModels.ToArray(),
                Abandonments = abandonedModels.ToArray()
            };

            return View(model);
        }

    }
}