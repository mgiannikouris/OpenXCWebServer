using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Data.DB
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Save changes without entity validation.
        /// </summary>
        /// <param name="context">The context to save changes on.</param>
        /// <returns>
        /// (from SaveChangesAsync) A task that represents the asynchronous save operation.  The task result
        /// contains the number of objects written to the underlying database.
        /// </returns>
        public static async Task<int> SaveChangesNoValidationAsync(this DbContext context)
        {
            bool oldValue = context.Configuration.ValidateOnSaveEnabled;
            context.Configuration.ValidateOnSaveEnabled = false;
            int result = await context.SaveChangesAsync();
            context.Configuration.ValidateOnSaveEnabled = oldValue;
            
            return result;
        }
    }
}
