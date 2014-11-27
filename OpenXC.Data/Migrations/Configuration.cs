namespace OpenXC.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using OpenXC.Data.DB;
    using OpenXC.Data.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    /// <summary>
    /// Configure database migrations.
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<OpenXCDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "OpenXC.Data.DB.OpenXCDbContext";
        }

        /// <summary>
        /// Seed the database context with some initial data.
        /// </summary>
        /// <param name="context">The context to add data to.</param>
        protected override void Seed(OpenXCDbContext context)
        {
            // Add a default user to the database.
            context.WebUsers.AddOrUpdate(dbWebUser => dbWebUser.UserName, new WebUser
            {
                UserName = "OpenXCAdmin",
                PasswordHash = new PasswordHasher().HashPassword("VXdDaBvdDU29rofs4Bmg")
            });
        }
    }
}
