namespace ServiceInsight.FunctionalTests.Services
{
    using DataBuilders;
    using Extensions;

    public class TestDataBuilder
    {
        public static EndpointListBuilder EndpointBuilder()
        {
            return new EndpointListBuilder();
        }

        public static MessageListBuilder MessageListBuilder(string endpoint = null)
        {
            return new MessageListBuilder(endpoint);
        }

        public static MessageBuilder MessageBuilder()
        {
            return new MessageBuilder();
        }

        public static ConversationBuilder ConversationBuilder()
        {
            return new ConversationBuilder();
        }
    }
}