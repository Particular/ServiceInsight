namespace ServiceInsight.SequenceDiagram.Drawing
{
    using System;

    public class HandlerViewModel : UmlViewModel
    {
        // Name (message received type + "handler" + DateTime)

        public HandlerStateType State { get; set; }

        public bool IsPartOfSaga => string.IsNullOrWhiteSpace(PartOfSaga);
        public string PartOfSaga { get; set; }//(saga name or null)

        public EndpointViewModel Endpoint { get; set; }
        public TimeSpan HandledAt { get; set; }

        public string Host { get; set; }
        public string Version { get; set; }

    }
}