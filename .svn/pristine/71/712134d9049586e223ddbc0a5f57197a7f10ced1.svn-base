using AutoMapper;
using OpenXC.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OpenXC.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // MVC Related config
            MvcHandler.DisableMvcResponseHeader = true;
            RouteTable.Routes.MapMvcAttributeRoutes();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            
            // Web API config.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Configure AutoMapper.
            Mapper.Initialize(config => config.AddProfile<AutoMapperWebProfile>());
            Mapper.AssertConfigurationIsValid();
        }
    }
}
