namespace ServiceInsight.SequenceDiagram.Diagram
{
    public class MessageProcessingRoute : DiagramItem
    {
        public MessageProcessingRoute(Arrow arrow, Handler processingHandler)
        {
            FromArrow = arrow;
            ProcessingHandler = processingHandler;
            Name = ProcessingHandler.Name + string.Format("({0})", FromArrow.MessageId);
            FromArrow.Route = this;
            ProcessingHandler.Route = this;
        }

        public Arrow FromArrow { get; }

        public Handler ProcessingHandler { get; }

        public override void OnIsFocusedChanged()
        {
            FromArrow.IsFocused = IsFocused;
            ProcessingHandler.IsFocused = IsFocused;
        }
    }

}