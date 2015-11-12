namespace ServiceInsight.SequenceDiagram.Diagram
{
    public class MessageProcessingRoute : DiagramItem
    {
        bool isFocused;

        public MessageProcessingRoute(Arrow arrow, Handler processingHandler)
        {
            FromArrow = arrow;
            ProcessingHandler = processingHandler;
            Name = ProcessingHandler.Name + string.Format("({0})", FromArrow.MessageId);
            FromArrow.Route = this;
            ProcessingHandler.Route = this;
        }

        public Arrow FromArrow { get; private set; }

        public Handler ProcessingHandler { get; private set; }

        public bool IsFocused
        {
            get { return isFocused; }
            set
            {
                if (isFocused == value)
                    return;

                isFocused = value;
                FromArrow.IsFocused = value;
                ProcessingHandler.IsFocused = value;
            }
        }
    }

}