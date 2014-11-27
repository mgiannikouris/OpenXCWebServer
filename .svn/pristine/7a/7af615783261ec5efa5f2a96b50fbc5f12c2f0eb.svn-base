using Microsoft.AspNet.Identity;
using OpenXC.Data.DB;
using OpenXC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace OpenXC.Data.UserStores
{
    /// <summary>
    /// User store for web users.
    /// </summary>
    public class WebUserStore :
        IUserStore<WebUser, int>,
        IUserPasswordStore<WebUser, int>,
        IQueryableUserStore<WebUser,int>
    {
        private readonly OpenXCDbContext db;
        private bool disposed = false;

        public WebUserStore(OpenXCDbContext dbContext)
        {
            db = dbContext;
        }

        public Task CreateAsync(WebUser user)
        {
            db.WebUsers.Add(user);
            return db.SaveChangesAsync();
        }

        public Task DeleteAsync(WebUser user)
        {
            db.WebUsers.Remove(user);
            return db.SaveChangesAsync();
        }

        public Task<WebUser> FindByIdAsync(int userId)
        {
            return db.WebUsers.FindAsync(userId);
        }

        public Task<WebUser> FindByNameAsync(string userName)
        {
            return db.WebUsers
                .SingleOrDefaultAsync(dbWebUser => dbWebUser.UserName == userName);
        }

        public Task UpdateAsync(WebUser user)
        {
            db.Entry(user).State = EntityState.Modified;
            return db.SaveChangesAsync();
        }

        public void Dispose()
        {
            disposed = true;
        }

        public Task<string> GetPasswordHashAsync(WebUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(WebUser user)
        {
            return Task.FromResult(!String.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(WebUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return db.SaveChangesAsync();
        }

        public IQueryable<WebUser> Users
        {
            get
            {
                return db.WebUsers;
            }
        }
    }
}