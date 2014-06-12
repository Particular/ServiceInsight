namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub
{
    using System.Web.Http.Filters;
    using Desktop.ServiceControl;

    public class VersionInformationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            actionExecutedContext.Response.Headers.Add(ServiceControlHeaders.ParticularVersion, "1.0.0-stub");
        }
    }
}