using System;

namespace NServiceBus.Profiler.Common.Models
{
    public class StoredMessage : MessageBody
    {
        public MessageStatus Status { get; set; }
        public Endpoint OriginatingEndpoint { get; set; }
        public Endpoint ReceivingEndpoint { get; set; }
        //public SagaDetails OriginatingSaga { get; set; }
        public bool IsDeferredMessage { get; set; }
        public string MessageType { get; set; }
        public string RelatedToMessageId { get; set; }
        public string ConversationId { get; set; }
    }

    public enum MessageStatus
    {
        Failed = 1,
        RepeatedFailures = 2,
        Successfull = 3
    }
}