namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using DataBuilders;
    using Desktop.Models;

    public class TestDataBuilder
    {
        public static EndpointBuilder EndpointBuilder()
        {
            return new EndpointBuilder();
        }

        public static MessageListBuilder MessageBuilder(Endpoint endpoint = null)
        {
            return new MessageListBuilder(endpoint);
        }
    }
}