namespace Particular.ServiceInsight.FunctionalTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using Desktop.Models;
    using TestData;

    public class MessageBuilder
    {
        string messageId;
        string conversationId;
        TimeSpan criticalTime;
        TimeSpan processingTime;
        TimeSpan deliveryTime;
        Type messageType;
        MessageStatus messageStatus;
        DateTime timeSent;
        List<StoredMessageHeader> headers;
        Endpoint sendingEndpoint;
        Endpoint receivingEndpoint;

        public MessageBuilder()
        {
            messageId = Guid.NewGuid().ToString();
            criticalTime = TimeSpan.FromSeconds(5);
            deliveryTime = TimeSpan.FromSeconds(4);
            processingTime = TimeSpan.FromSeconds(3);
            messageType = typeof(SubmitOrder);
            messageStatus = MessageStatus.Successful;
            timeSent = DateTime.Now.AddMinutes(-5);
            conversationId = Guid.NewGuid().ToString();
            headers = new List<StoredMessageHeader>();
            receivingEndpoint = null;
            sendingEndpoint = null;
        }

        public MessageBuilder WithMessageId(string value)
        {
            messageId = value;
            return this;
        }

        public MessageBuilder WithCriticalTime(TimeSpan value)
        {
            criticalTime = value;
            return this;
        }

        public MessageBuilder WithProcessingTime(TimeSpan value)
        {
            processingTime = value;
            return this;
        }

        public MessageBuilder WithDeliveryTime(TimeSpan value)
        {
            deliveryTime = value;
            return this;
        }

        public MessageBuilder WithMessageType(Type value)
        {
            messageType = value;
            return this;
        }

        public MessageBuilder WithMessageStatus(MessageStatus value)
        {
            messageStatus = value;
            return this;
        }

        public MessageBuilder WithTimeSent(DateTime value)
        {
            timeSent = value;
            return this;
        }

        public MessageBuilder WithConversationId(string value)
        {
            conversationId = value;
            return this;
        }

        public MessageBuilder WithRelatedMessageId(string value)
        {
            headers.Add(new StoredMessageHeader { Key = MessageHeaderKeys.RelatedTo, Value = value });
            return this;
        }

        public MessageBuilder WithSendingEndpoint(Endpoint value)
        {
            sendingEndpoint = value;
            return this;
        }

        public MessageBuilder WithReceivingEndpoint(Endpoint value)
        {
            receivingEndpoint = value;
            return this;
        }

        public StoredMessage Build()
        {
            return new StoredMessage
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = messageId,
                CriticalTime = criticalTime,
                DeliveryTime = deliveryTime,
                ProcessingTime = processingTime,
                MessageType = messageType.FullName,
                Status = messageStatus,
                TimeSent = timeSent,
                ConversationId = conversationId,
                SendingEndpoint = sendingEndpoint,
                ReceivingEndpoint = receivingEndpoint,
                Headers = headers,
            };
        }
    }
}