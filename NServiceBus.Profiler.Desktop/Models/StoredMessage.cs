using System.Diagnostics;

namespace NServiceBus.Profiler.Desktop.Models
{
    [DebuggerDisplay("Id={Id},MessageId={MessageId},RelatedToMessageId={RelatedToMessageId}")]
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
        public string ContentType { get; set; }
        public string MessageId { get; set; }
    }
}