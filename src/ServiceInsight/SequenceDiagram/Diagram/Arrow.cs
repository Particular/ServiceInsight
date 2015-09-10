namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Diagnostics;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem
    {
        public Handler FromHandler { get; set; }

        public Handler ToHandler { get; set; }

        public ArrowType Type { get; set; }
    }
}