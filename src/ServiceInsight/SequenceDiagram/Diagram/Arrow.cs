namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Diagnostics;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem
    {
        private Handler fromHandler;
        public Handler FromHandler
        {
            get { return fromHandler; }
            set
            {
                fromHandler = value;
                OnPropertyChanged("FromHandler");
            }
        }

        private Handler toHandler;
        public Handler ToHandler
        {
            get { return toHandler; }
            set
            {
                toHandler = value;
                OnPropertyChanged("ToHandler");
            }
        }

        public ArrowType Type { get; set; }
    }
}