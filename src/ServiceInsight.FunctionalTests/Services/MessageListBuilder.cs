namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using System.Collections.Generic;
    using Desktop.Models;

    public class MessageListBuilder
    {
        private List<StoredMessage> messagesList;

        public MessageListBuilder()
        {
            messagesList = new List<StoredMessage>();
        }

        public MessageListBuilder WithMessages(params StoredMessage[] messages)
        {
            messagesList.AddRange(messages);
            return this;
        }

        public void Build()
        {
            TestDataWriter.Write("Messages", messagesList);
        }
    }
}