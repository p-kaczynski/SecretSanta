using System.Web.Mvc;
using AutoMapper;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserRepository _userRepository;

        public RegistrationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(new SantaUserPostModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SantaUserPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // save to db
            var domainModel = Mapper.Map<SantaUser>(model);
            _userRepository.InsertUser(domainModel);
            // TODO: change to confirmaton
            return RedirectToAction(nameof(Confirmation), new {domainModel});
        }

        public ActionResult Confirmation(SantaUser model)
        {
            if (model == null)
                return HttpNotFound();

            return View(model);
        }
    }
}