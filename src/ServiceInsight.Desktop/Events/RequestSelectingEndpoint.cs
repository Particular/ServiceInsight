namespace Particular.ServiceInsight.Desktop.Events
{
    using Models;

    public class RequestSelectingEndpoint
    {
        public Endpoint Endpoint { get; private set; }

        public RequestSelectingEndpoint(Endpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}