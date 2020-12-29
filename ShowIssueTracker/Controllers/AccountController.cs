using ShowIssueTracker.Authorization;
using ShowIssueTracker.Data;
using ShowIssueTracker.Models;
using ShowIssueTracker.Models.AccountViewModels;
using ShowIssueTracker.Repositories;
using ShowIssueTracker.Services;
using ShowIssueTracker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShowIssueTracker.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
         private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;       
        private readonly ILogger _logger;

        private readonly IImpexiumLogin _impexiumLogin;      

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,          
            ILoggerFactory loggerFactory,
            IImpexiumLogin impexiumLogin)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _impexiumLogin = impexiumLogin;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Profile()
        {
           return View();
        }
        [HttpGet]
        public ActionResult Redirect()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true

            var impexiumLogin = await _impexiumLogin.UserLoginToken(model);

            if (!impexiumLogin.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Impexium login attempt. Or you have no access to this interface, Please contact  hmartinez@nada.org  to get access to the interface.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            await _signInManager.SignInAsync(user, isPersistent: false);
            //var result = await _signInManager.PasswordSignInAsync(user.Email, user.Email.ToLower(), model.RememberMe, lockoutOnFailure: false);
            //if (!result.Succeeded)
            //{
            //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //    return View(model);
            //}
            _logger.LogInformation(1, $"User {model.Email} logged in.");
            return RedirectToLocal(returnUrl);
        }     

       
        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(Login));
        }       

      
        //
        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");//RedirectToAction(nameof(JobRequestsController.Index), "JobRequests");
            }
        }

        #endregion
    }
}