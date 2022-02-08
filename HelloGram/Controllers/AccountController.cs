using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HelloGram.Core.Models;
using HelloGram.Core.ViewModels;

namespace HelloGram.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByEmailAsync(model.Email);

            // Uncomment when using a secure domain
            //if (user != null)
            //{
            //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //    {
            //        var callbackUrl = await SendEmailConfirmationTokenAsync(user.Id);

            //        ViewBag.ErrorMessage = "Please, verify your email first."
            //                               + "\nThe confirmation token has been resent to your email account.";
            //        return View("Error");
            //    }
            //}

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Error");
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.IsAgreed)
                {
                    ModelState.AddModelError("", "You must agree to the Privacy Policy & the Terms of Service before you proceed.");
                    return View(model);
                }

                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Name = model.Name, CreatedAt = DateTime.UtcNow };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //var callbackUrl = await SendEmailConfirmationTokenAsync(user.Id);

                    // Uncomment to debug locally 
                    // TempData["ViewBagLink"] = callbackUrl;

                    // Uncomment when using a secure domain

                    //ViewBag.Title = "Email Verification";
                    //ViewBag.Message = "Check your email and confirm your account, you must be confirmed before you can log in."
                    //                    + "\nOr Log in if the message has not been sent for resending it.";

                    ViewBag.Title = "Account Has Been Created";
                    ViewBag.Message = "You can log in now with your account";

                    //ViewBag.Title = "Registration Succeed";
                    //ViewBag.Message = "We've created your account successfully, and you can login now.";

                    return View("Info");
                    //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.Title = "Email Verification";
                    ViewBag.Message = "Please, check your email to reset your password.";

                    return View("Info");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link

                // Uncomment when using a secure domain
                //var callbackUrl = await SendEmailResetTokenAsync(user.Id);

                //ViewBag.Title = "Email Verification";
                //ViewBag.Message = "Please, check your email to reset your password.";

                ViewBag.Title = "Demo Feature";
                ViewBag.Message = "Unfortunately, this feature is not currently supported due to the insecure domain.";

                return View("Info");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private const string MessageBody = "<div bgcolor=\"#ffffff\"> <table width=\"500px\" align=\"center\"> <tbody> <tr> <td> <p align=\"center\"> </p> <p>###</p> <h3> <a href=\"@@@\" target=\"_blank\" style=\"text-align: center;\">Confirm Now</a> </h3> <p>We are really happy for you to join our community. <p>Regards, <br> <strong>The Hellogram Team</strong> <br> </p> </td> </tr> </tbody> </table></div>";
        private async Task<string> SendEmailResetTokenAsync(string userId)
        {
            var code = await UserManager.GeneratePasswordResetTokenAsync(userId);
            if (Request.Url != null)
            {
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
                var content = MessageBody
                    .Replace("###",
                        "Thanks for trusting us at HelloGram. Please click this link to reset your password:")
                    .Replace("@@@", callbackUrl);
                await UserManager.SendEmailAsync(userId, "Reset your Password", content);

                return callbackUrl;
            }

            return "Error";
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userId)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            if (Request.Url != null)
            {
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
                var content = MessageBody
                    .Replace("###",
                        "Thanks for registering with HelloGram. Please click this link to confirm your Email:")
                    .Replace("@@@", callbackUrl);
                await UserManager.SendEmailAsync(userId, "Confirm your Email", content);

                return callbackUrl;
            }

            return "Error";
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}