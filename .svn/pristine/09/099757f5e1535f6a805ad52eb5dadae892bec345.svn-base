using System.Web;
using System.Web.Mvc;

namespace OpenXC.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
            // Controller actions must explicitly opt-out of authorization.
            filters.Add(new AuthorizeAttribute());
        }
    }
}
