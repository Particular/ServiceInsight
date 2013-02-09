using System;

namespace NServiceBus.Profiler.Common.Models
{
    public class TransportMessage
    {
        public byte[] BodyRaw { get; set; }
        public DateTime TimeSent { get; set; }
        public string Id { get; set; }
        public string MessageType { get; set; }
        public string Body { get; set; }
        public string RelatedToMessageId { get; set; }
        public MessageStatus Status { get; set; }
        public Endpoint OriginatingEndpoint { get; set; }
        public Endpoint ReceivingEndpoint { get; set; }
        //public SagaDetails OriginatingSaga { get; set; }
    }

    public enum MessageStatus
    {
        Failed = 1,
        RepeatedFailures = 2,
        Successfull = 3
    }
}