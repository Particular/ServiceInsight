namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Desktop.ServiceControl;

    public abstract class ControllerBase : ApiController
    {
        protected HttpResponseMessage Response<T>(IList<T> result)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, result);
            
            response.Headers.Add(DefaultServiceControl.ServiceControlHeaders.TotalCount, result.Count.ToString(CultureInfo.InvariantCulture));

            return response;
        }
    }
}