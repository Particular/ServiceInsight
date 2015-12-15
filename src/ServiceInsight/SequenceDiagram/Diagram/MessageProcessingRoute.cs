namespace ServiceInsight.SequenceDiagram.Diagram
{
    public class MessageProcessingRoute : DiagramItem
    {
        public MessageProcessingRoute(Arrow arrow, Handler processingHandler)
        {
            FromArrow = arrow;
            ProcessingHandler = processingHandler;

            if (FromArrow != null && ProcessingHandler != null)
            {
                Name = ProcessingHandler.Name + string.Format("({0})", FromArrow.MessageId);
            }

            if (FromArrow != null)
            {
                FromArrow.Route = this;
            }

            if (ProcessingHandler != null)
            {
                ProcessingHandler.Route = this;
            }
        }

        public Arrow FromArrow { get; }

        public Handler ProcessingHandler { get; }

        protected override void OnIsFocusedChanged()
        {
            FromArrow.IsFocused = IsFocused;
            ProcessingHandler.IsFocused = IsFocused;
        }
    }
}