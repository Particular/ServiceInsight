namespace ServiceInsight.Framework.Events
{
    using ServiceInsight.Models;

    public class RequestSelectingEndpoint
    {
        public Endpoint Endpoint { get; }

        public RequestSelectingEndpoint(Endpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}