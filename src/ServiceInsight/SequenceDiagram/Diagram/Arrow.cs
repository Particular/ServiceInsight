namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem, IComparable<Arrow>
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

        public DateTime? SentTime { get; set; }

        public string MessageId
        {
            get { return messageId; }
        }

        public double Width { get; set; }

        public int CompareTo(Arrow other)
        {
            if (!other.SentTime.HasValue && !SentTime.HasValue)
            {
                return 0;
            }

            if (SentTime.HasValue && !other.SentTime.HasValue)
            {
                return 1;
            }

            if (!SentTime.HasValue)
            {
                return -1;
            }

            return SentTime.Value.CompareTo(other.SentTime.Value);
        }
    }
}