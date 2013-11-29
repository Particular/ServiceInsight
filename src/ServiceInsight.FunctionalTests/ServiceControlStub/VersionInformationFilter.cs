using System.Web.Http.Filters;
using NServiceBus.Profiler.Desktop.ServiceControl;

namespace NServiceBus.Profiler.FunctionalTests.ServiceControlStub
{
    public class VersionInformationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            actionExecutedContext.Response.Headers.Add(DefaultServiceControl.ServiceControlHeaders.ParticularVersion, "1.0.0-stub");
        }
    }
}