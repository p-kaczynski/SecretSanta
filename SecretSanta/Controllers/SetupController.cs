using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.SecurityModels;

namespace SecretSanta.Controllers
{
    public class SetupController : Controller
    {
        private readonly bool _devMode;
        private readonly UserManager<SantaSecurityUser, string> _userManager;
        private readonly ISantaAdminProvider _adminProvider;
        private readonly IEncryptionProvider _encryptionProvider;

        public SetupController(IConfigProvider configProvider, UserManager<SantaSecurityUser, string> userManager, ISantaAdminProvider adminProvider, IEncryptionProvider encryptionProvider)
        {
            _userManager = userManager;
            _adminProvider = adminProvider;
            _encryptionProvider = encryptionProvider;
            _devMode = configProvider.DevMode;
        }

        public async Task<ActionResult> Index()
        {
            if (!_devMode)
                return RedirectToAction("Index", "Home");

            // See if we need to create default admin account
            if (!_adminProvider.GetAllAdmins().Any())
                await CreateDefaultAdminAsync();

            return View();
        }

        private Task CreateDefaultAdminAsync()
        {
            var santaAdmin = new SantaAdmin
            {
                UserName = "admin",
                PasswordHash = _encryptionProvider.CalculatePasswordHash("admin")
            };
            return _userManager.CreateAsync(santaAdmin);
        }
    }
}