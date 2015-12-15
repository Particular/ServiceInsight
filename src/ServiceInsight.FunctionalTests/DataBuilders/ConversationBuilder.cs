namespace ServiceInsight.FunctionalTests.DataBuilders
{
    using System.Collections.Generic;
    using ServiceInsight.Models;
    using Services;

    public class ConversationBuilder
    {
        List<StoredMessage> messagesList;

        public ConversationBuilder()
        {
            messagesList = new List<StoredMessage>();
        }

        public ConversationBuilder WithMessages(params StoredMessage[] messages)
        {
            messagesList.AddRange(messages);
            return this;
        }

        public void Build()
        {
            TestDataWriter.Write("Conversation-Messages", messagesList);
        }
    }
}