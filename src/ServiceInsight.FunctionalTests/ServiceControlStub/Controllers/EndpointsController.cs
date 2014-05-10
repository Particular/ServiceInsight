namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Desktop.Models;

    public class EndpointsController : ApiController
    {
        public IEnumerable<Endpoint> Get()
        {
            return new[]
            {
                new Endpoint()
                {
                    Name = "Test",
                    HostDisplayName = "localhost",
                }
            };
        }
    }
}