using System.Linq;
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
    [Authorize(Roles = SantaUserManager.AdminRole)]
    public class AdminController : BaseController
    {
        private readonly UserManager<SantaSecurityUser, string> _userManager;
        private readonly ISantaAdminProvider _adminProvider;

        public AdminController(UserManager<SantaSecurityUser, string> userManager, ISantaAdminProvider adminProvider)
        {
            _userManager = userManager;
            _adminProvider = adminProvider;
        }

        public ActionResult Index()
        {
            var allAdmins = _adminProvider.GetAllAdmins().Select(admin=>new SantaAdminViewModel{Id = admin.AdminId, UserName = admin.UserName}).ToList();

            return View(allAdmins);
        }

        public ActionResult CreateAdmin()
        {
            return View("EditAdmin", new SantaAdminPostModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAdmin(SantaAdminPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditAdmin", model);
            }
            var santaAdmin = Mapper.Map<SantaAdmin>(model);
            var result = await _userManager.CreateAsync(santaAdmin);
            if (result.Succeeded)
                RedirectToAction("Index", "Home");

            ModelState.AddModelError("", string.Join("<br />", result.Errors));
            return View("EditAdmin", model);
        }

        [HttpGet]
        public async Task<ActionResult> EditAdmin(int id)
        {
            var userId = SantaSecurityUser.GetId(id, true);
            var santaAdmin = await _userManager.FindByIdAsync(userId);
            if (santaAdmin == null)
                return HttpNotFound();
            var santaAdminPostModel = Mapper.Map<SantaAdminPostModel>(santaAdmin);
            return View(santaAdminPostModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAdmin(SantaAdminPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditAdmin", model);
            }
            var santaAdmin = Mapper.Map<SantaAdmin>(model);
            var result = await _userManager.UpdateAsync(santaAdmin);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", string.Join("<br />", result.Errors));
            return View(model);
        }
    }
}