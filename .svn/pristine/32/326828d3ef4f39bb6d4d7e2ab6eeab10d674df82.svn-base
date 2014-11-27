using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using OpenXC.Services.Devices;
using System.Diagnostics;
using OpenXC.Services.Upgrades;

namespace OpenXC.Web.Filters
{
    /// <summary>
    /// ExceptionFilterAttribute to handle Device API specific exceptions.
    /// </summary>
    public class DeviceApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // Trace log the exception.
            Trace.TraceError(context.Exception.ToString());

            if (context.Exception is DeviceNotFoundException ||
                context.Exception is UpgradeNotFoundException)
            {
                // The device queried was not found.
                context.Response = context.Request.CreateErrorResponse(
                    System.Net.HttpStatusCode.NotFound,
                    context.Exception.Message);
            }
            else
            {
                base.OnException(context);
            }
        }
    }
}