namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub.Controllers
{
    using System;
    using System.Net.Http;
    using Desktop.Models;

    public class MessagesController : ControllerBase
    {
        public HttpResponseMessage Get()
        {
            return Response(new[]
            {
                new StoredMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Status = MessageStatus.Successful
                }
            });
        }
    }
}