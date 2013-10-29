using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace NServiceBus.Profiler.FunctionalTests.ServiceControlStub
{
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