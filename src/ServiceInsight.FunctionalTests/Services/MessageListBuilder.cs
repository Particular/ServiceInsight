namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using System.Collections.Generic;
    using Desktop.Models;

    public class MessageListBuilder
    {
        List<StoredMessage> messagesList;
        Endpoint endpoint;

        public MessageListBuilder(Endpoint endpoint = null)
        {
            this.endpoint = endpoint;
            messagesList = new List<StoredMessage>();
        }

        public MessageListBuilder WithMessages(params StoredMessage[] messages)
        {
            messagesList.AddRange(messages);
            return this;
        }

        public void Build()
        {
            var outputFile = endpoint != null ? string.Format("{0}-Messages", endpoint.Name) : "Messages";
            TestDataWriter.Write(outputFile, messagesList);
        }
    }
}