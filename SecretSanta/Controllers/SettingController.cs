using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            //Mapper.Map<SettingsViewModel>(_settingsRepository)
            var model = new SettingsViewModel{RegistrationOpen = _settingsRepository.RegistrationOpen};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloseRegistration()
        {
            _settingsRepository.RegistrationOpen = false;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpenRegistration()
        {
            _settingsRepository.RegistrationOpen = true;
            return RedirectToAction("Index");
        }
    }
}