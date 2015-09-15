namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Diagnostics;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem
    {
        readonly string messageId;

        public Arrow(string messageId)
        {
            this.messageId = messageId;
        }

        public Handler FromHandler { get; set; }

        public Handler ToHandler { get; set; }

        public Direction Direction { get; set; }

        public ArrowType Type { get; set; }

        public string MessageId
        {
            get { return messageId; }
        }

        public double Width { get; set; }
    }
}