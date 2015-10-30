namespace ServiceInsight.SequenceDiagram.Diagram
{
    public class MessageProcessingRoute : DiagramItem
    {
        public MessageProcessingRoute(Arrow arrow, Handler processingHandler)
        {
            FromArrow = arrow;
            ProcessingHandler = processingHandler;
            Name = ProcessingHandler.Name + string.Format("({0})", FromArrow.MessageId);
        }

        public Arrow FromArrow { get; private set; }
        public Handler ProcessingHandler { get; private set; }
    }

}