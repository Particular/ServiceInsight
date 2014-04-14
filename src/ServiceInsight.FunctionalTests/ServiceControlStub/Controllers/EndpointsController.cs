using System.Collections.Generic;
using System.Web.Http;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.FunctionalTests.ServiceControlStub.Controllers
{
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