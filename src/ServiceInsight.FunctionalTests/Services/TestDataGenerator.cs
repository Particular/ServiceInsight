namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using DataBuilders;

    public class TestDataBuilder
    {
        public static EndpointListBuilder EndpointBuilder()
        {
            return new EndpointListBuilder();
        }

        public static MessageListBuilder MessageBuilder(string endpoint = null)
        {
            return new MessageListBuilder(endpoint);
        }
    }
}