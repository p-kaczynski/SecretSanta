using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Caching;
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
        private const string AssignmentCacheKey = "secret_santa.last_assignment_result";

        private readonly IAssignmentService _assignmentService;
        private readonly IEmailService _emailService;

        public AssignmentController(IEmailService emailService, IAssignmentService assignmentService)
        {
            _emailService = emailService;
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (_assignmentService.WasAssigned())
            {
                return View(_assignmentService.GetAssignments());
            }
            return View((AssignmentResult)null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign()
        {
            if (_assignmentService.WasAssigned())
                return RedirectToAction("Index");

            // remove any previous
            HttpContext.Cache.Remove(AssignmentCacheKey);

            var assignments = _assignmentService.GenerateAssignments();
            // Save to cache
            HttpContext.Cache.Add(AssignmentCacheKey, assignments, null, Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

            return View("AssignmentGenerated", assignments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveLastAssignment()
        {
            var assignments = (AssignmentResult) HttpContext.Cache.Get(AssignmentCacheKey);
            if (assignments == null)
                return View("Message",
                    model: Resources.Global.Assignments_NoAssignmentsInCache);
            
            _assignmentService.SaveAssignments(assignments);
            return View("AssignmentCompleted", assignments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendAssignmentEmails()
        {
            if (!_assignmentService.WasAssigned())
                return RedirectToAction("Index");

            var assignments = _assignmentService.GetAssignments();
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