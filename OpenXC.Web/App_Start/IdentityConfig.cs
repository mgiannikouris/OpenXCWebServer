using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using OpenXC.Web.Models;
using OpenXC.Data.Models;
using OpenXC.Data.DB;
using OpenXC.Data.UserStores;

namespace OpenXC.Web
{
    public class WebUserManager : UserManager<WebUser, int>
    {
        public WebUserManager(IUserStore<WebUser, int> store)
            : base(store)
        {
        }

        public static WebUserManager Create(IdentityFactoryOptions<WebUserManager> options, IOwinContext context)
        {
            var manager = new WebUserManager(new WebUserStore(context.Get<OpenXCDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<WebUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            return manager;
        }
    }
}
