namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Handled '{Name}' and resulted in {State}")]
    public class Handler : DiagramItem
    {
        public Handler()
        {
            Out = new List<Arrow>();
        }

        public EndpointItem Endpoint { get; set; }

        public override double X
        {
            get { return Endpoint.X; }
            set
            {
                //no setter needed
            }
        }

        public bool IsPartOfSaga
        {
            get { return PartOfSaga != null; }
        }

        public bool IsHighlighted { get; set; }

        public HandlerState State { get; set; }
        public string PartOfSaga { get; set; }
        public Arrow In { get; set; }
        public List<Arrow> Out { get; set; }
        public DateTime? HandledAt { get; set; }


    }
}