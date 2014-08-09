namespace Particular.ServiceInsight.FunctionalTests.DataBuilders
{
    using System.Collections.Generic;
    using Desktop.Models;
    using Services;

    public class MessageListBuilder
    {
        List<StoredMessage> messagesList;
        string endpoint;

        public MessageListBuilder(string endpoint = null)
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
            var outputFile = endpoint != null ? string.Format("{0}-Messages", endpoint) : "Messages";
            TestDataWriter.Write(outputFile, messagesList);
        }
    }
}