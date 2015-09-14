namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Handled '{Name}' and resulted in {State}")]
    public class Handler : DiagramItem, IComparable<Handler>
    {
        readonly string id;
        Arrow arrowIn;

        public Handler(string id)
        {
            this.id = id;
            Out = new List<Arrow>();
        }

        public EndpointItem Endpoint { get; set; }

        public bool IsPartOfSaga
        {
            get { return PartOfSaga != null; }
        }

        public HandlerState State { get; set; }
        public string PartOfSaga { get; set; }

        public Arrow In
        {
            get { return arrowIn; }
            set
            {
                if (arrowIn != null)
                {
                    throw new Exception("Only one arrow is allowed to come in");
                }

                arrowIn = value;
            }
        }

        public List<Arrow> Out { get; set; }
        public DateTime? HandledAt { get; set; }

        public string ID
        {
            get { return id; }
        }

        public int CompareTo(Handler other)
        {
            if (!other.HandledAt.HasValue && !HandledAt.HasValue)
            {
                return 0;
            }

            if (HandledAt.HasValue && !other.HandledAt.HasValue)
            {
                return 1;
            }

            if (!HandledAt.HasValue)
            {
                return -1;
            }

            return HandledAt.Value.CompareTo(other.HandledAt.Value);
        }
    }
}