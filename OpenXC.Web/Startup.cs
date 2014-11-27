using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenXC.Web.Startup))]
namespace OpenXC.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
