namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using DataBuilders;

    public class TestDataBuilder
    {
        public static EndpointBuilder EndpointBuilder()
        {
            return new EndpointBuilder();
        }

        public static MessageListBuilder MessageBuilder()
        {
            return new MessageListBuilder();
        }
    }
}