namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Input;
    using Particular.ServiceInsight.Desktop.Models;

    [DebuggerDisplay("Handled '{Name}' and resulted in {State}")]
    public class Handler : DiagramItem, IComparable<Handler>
    {
        readonly string id;
        Arrow arrowIn;
        bool isFocused;

        public Handler(string id, IMessageCommandContainer container)
        {
            this.id = id;
            this.ChangeCurrentMessage = container.ChangeSelectedMessageCommand;
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
        public TimeSpan? ProcessingTime { get; set; }

        public Direction EffectiveArrowDirection
        {
            get
            {
                if (Out.Count > 1)
                {
                    if(Out[0].Direction == Direction.Left) return Direction.Right;
                    return Direction.Left;
                }

                return Direction.Left;
            }
        }

        public string ID
        {
            get { return id; }
        }

        public bool IsFocused
        {
            get { return isFocused; }
            set
            {
                if (isFocused == value)
                    return;

                isFocused = value;
                Route.IsFocused = value;
                NotifyOfPropertyChange(() => IsFocused);
            }
        }

        public StoredMessage SelectedMessage
        {
            get
            {
                return Route?.FromArrow.SelectedMessage;
            }
        }

        public MessageProcessingRoute Route { get; set; }

        public ICommand ChangeCurrentMessage { get; set; }

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