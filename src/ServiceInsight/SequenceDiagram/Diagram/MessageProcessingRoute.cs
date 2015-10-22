namespace ServiceInsight.SequenceDiagram.Diagram
{
    public class MessageProcessingRoute : DiagramItem
    {
        public MessageProcessingRoute(Arrow arrow, Handler processingHandler)
        {
            FromArrow = arrow;
            ProcessingHandler = processingHandler;
        }

        public Arrow FromArrow { get; private set; }
        public Handler ProcessingHandler { get; private set; }
    }

}