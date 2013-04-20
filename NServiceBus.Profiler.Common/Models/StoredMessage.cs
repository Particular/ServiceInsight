using System.Diagnostics;

namespace NServiceBus.Profiler.Common.Models
{
    [DebuggerDisplay("Id={Id}, RelatedToMessageId={RelatedToMessageId}")]
    public class StoredMessage : MessageBody
    {
        public MessageStatus Status { get; set; }
        public FailureDetails FailureDetails { get; set; }
        public Endpoint OriginatingEndpoint { get; set; }
        public Endpoint ReceivingEndpoint { get; set; }
        //public SagaDetails OriginatingSaga { get; set; }
        public MessageStatistics Statistics { get; set; }
        public bool IsDeferredMessage { get; set; }
        public string RelatedToMessageId { get; set; }
        public string ConversationId { get; set; }
    }
}