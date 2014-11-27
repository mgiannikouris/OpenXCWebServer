using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OpenXC.Data.Models;
using OpenXC.Web.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenXC.Web.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private WebUserManager _userManager;
        public WebUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<WebUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Home base for OpenXC website.
        /// </summary>
        [Route("")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Login page.
        /// </summary>
        [AllowAnonymous]
        [Route("login")]
        [HttpGet]
        public ActionResult LogIn()
        {
            return View(new LoginViewModel());
        }

        /// <summary>
        /// Login form submit.
        /// </summary>
        /// <param name="model"></param>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(WebUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Logout link.
        /// </summary>
        [AllowAnonymous]
        [Route("logout")]
        [HttpGet]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index");
        }
    }
}