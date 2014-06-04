namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub.Controllers
{
    using System.Net.Http;
    using Desktop.Models;

    public class EndpointsController : ControllerBase
    {
        public HttpResponseMessage Get()
        {
            return Response(new[]
            {
                new Endpoint
                {
                    Name = "Test",
                    HostDisplayName = "localhost",
                }
            });
        }

    }
}