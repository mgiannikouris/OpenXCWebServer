using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using OpenXC.Data.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using OpenXC.Web.Models;

namespace OpenXC.Web.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : Controller
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

        // GET: Users
        [Route("")]
        public async Task<ActionResult> Index()
        {
            List<WebUser> webUsers = await UserManager.Users.ToListAsync();
            return View(webUsers.Select(dbWebUser => new WebUserViewModel
            {
                UserId = dbWebUser.Id,
                UserName = dbWebUser.UserName
            })
            .ToList());
        }

        [Route("create")]
        [HttpGet]
        public ActionResult Create()
        {
            return View(new WebUserViewModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<ActionResult> Create(WebUserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(new WebUser { UserName = newUser.UserName }, newUser.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("UserCreationError", error);
                    }
                }
            }

            // There must be model errors.
            return View(newUser);
        }

        [Route("{userId}/password")]
        [HttpGet]
        public async Task<ActionResult> ChangePassword(int userId)
        {
            WebUser webUser = await UserManager.FindByIdAsync(userId);
            if (webUser == null)
            {
                return HttpNotFound();
            }

            return View(new ChangePasswordViewModel
            {
                UserName = webUser.UserName
            });
        }

        [Route("{userId}/password")]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(int userId, FormCollection form)
        {
            ChangePasswordViewModel passwordChange = new ChangePasswordViewModel();
            if (TryUpdateModel(passwordChange))
            {
                var result = await UserManager.ChangePasswordAsync(userId, passwordChange.CurrentPassword, passwordChange.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("PasswordChangeError", error);
                    }
                }
            }

            return await ChangePassword(userId);
        }

        [Route("{userId}/delete")]
        [HttpGet]
        public async Task<ActionResult> Delete(int userId)
        {
            WebUser webUser = await UserManager.FindByIdAsync(userId);
            if (webUser == null)
            {
                return HttpNotFound();
            }

            return View(new WebUserViewModel
            {
                UserId = webUser.Id,
                UserName = webUser.UserName
            });
        }

        [Route("{userId}/delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(int userId, FormCollection form)
        {
            WebUser webUser = await UserManager.FindByIdAsync(userId);
            if (webUser == null)
            {
                return HttpNotFound();
            }

            // Don't delete the current user!
            if (String.Equals(webUser.UserName, User.Identity.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("UserDeletionError", "Cannot delete the currently logged in user.");
            }
            else
            {
                var result = await UserManager.DeleteAsync(webUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("UserDeletionError", error);
                    }
                }
            }

            return View(new WebUserViewModel
            {
                UserId = webUser.Id,
                UserName = webUser.UserName
            });
        }
    }
}