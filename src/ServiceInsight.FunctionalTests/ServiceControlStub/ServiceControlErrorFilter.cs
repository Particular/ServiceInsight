namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;

    public class ServiceControlErrorFilter : IExceptionFilter 
    {
        public bool AllowMultiple { get { return false; } }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            return AsyncTask.DefaultCompleted;
        }
    }
}