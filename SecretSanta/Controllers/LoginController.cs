﻿using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Resources;
using SecretSanta.Models;
using SecretSanta.Security;

namespace SecretSanta.Controllers
{
    [Authorize]
    public class LoginController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var status = await HttpContext.GetOwinContext().Get<SantaSignInManager>().PasswordSignInAsync(model.Login, model.Password, true, false);
            switch (status)
            {
                case SignInStatus.Success:
                    return RedirectToAction(nameof(Index), "Home");
                case SignInStatus.LockedOut:
                    ModelState.AddModelError(Constants.GlobalModerError, Global.Login_LockedOut);
                    return View(model);
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("TODO");
                case SignInStatus.Failure:
                    ModelState.AddModelError(Constants.GlobalModerError, Global.Login_Invalid);
                    return View(model);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication
                .SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

    }
}