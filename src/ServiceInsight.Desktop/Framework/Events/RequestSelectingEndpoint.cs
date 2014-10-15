namespace Particular.ServiceInsight.Desktop.Framework.Events
{
    using Particular.ServiceInsight.Desktop.Models;

    public class RequestSelectingEndpoint
    {
        public Endpoint Endpoint { get; private set; }

        public RequestSelectingEndpoint(Endpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}