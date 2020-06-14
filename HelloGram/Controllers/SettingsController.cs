using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core.Models;
using HelloGram.Core.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace HelloGram.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public SettingsController()
        {
        }

        public SettingsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(int updated = 0)
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.Updated = updated;

            return View(new SettingsViewModel(user));
        }

        [HttpPost]
        public async Task<ActionResult> Index(SettingsViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            model.UserViewModel.Update(user);
            
            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ViewBag.AccountSuccess = true;
                return RedirectToAction("Index", new { updated = 1 });
            }
            AddErrors(result);
            ViewBag.AccountSuccess = false;
            return View(model);
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePassword(SettingsViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            var result = await UserManager.ChangePasswordAsync(userId, model.ChangePasswordViewModel.OldPassword, model.ChangePasswordViewModel.NewPassword);
            if (result.Succeeded)
            {
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                
                return RedirectToAction("Index", new { updated = 1 });
            }
            AddErrors(result);
            var newModel = new SettingsViewModel(user) { ChangePasswordViewModel = model.ChangePasswordViewModel };
            ViewBag.SecuritySuccess = false;
            return View("Index", newModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion
    }
}