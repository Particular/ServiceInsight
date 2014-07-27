namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using System.Collections.Generic;
    using Desktop.Models;

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