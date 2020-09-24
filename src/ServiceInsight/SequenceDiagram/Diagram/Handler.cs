namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;
    using ServiceInsight.Models;

    [DebuggerDisplay("Handled '{Name}' and resulted in {State} at {HandledAt}")]
    public class Handler : DiagramItem, IComparable<Handler>
    {
        readonly string id;
        Arrow arrowIn;

        public Handler(string id, IMessageCommandContainer container)
        {
            this.id = id;
            ChangeCurrentMessage = container?.ChangeSelectedMessageCommand;
            Out = new List<Arrow>();
        }

        public EndpointItem Endpoint { get; set; }

        public bool IsPartOfSaga => PartOfSaga != null;

        public HandlerState State { get; set; }

        public string PartOfSaga { get; set; }

        public Arrow In
        {
            get => arrowIn;

            set
            {
                if (arrowIn != null)
                {
                    throw new Exception("Only one arrow is allowed to come in");
                }

                arrowIn = value;
            }
        }

        public IEnumerable<Arrow> Out { get; set; }

        public DateTime? ProcessedAt { get; set; }

        DateTime? processedAtGuess;

        public void UpdateProcessedAtGuess(DateTime? timeSent)
        {
            if (!timeSent.HasValue)
            {
                return;
            }

            if (!processedAtGuess.HasValue || processedAtGuess.Value > timeSent.Value)
            {
                processedAtGuess = timeSent;
            }
        }

        public DateTime? HandledAt => ProcessedAt ?? processedAtGuess;

        public TimeSpan? ProcessingTime { get; set; }

        public Direction EffectiveArrowDirection => Out?.FirstOrDefault()?.Direction == Direction.Left ? Direction.Right : Direction.Left;

        public string ID => id;

        protected override void OnIsFocusedChanged()
        {
            if (Route != null)
            {
                Route.IsFocused = IsFocused;
            }
        }

        public StoredMessage SelectedMessage => Route?.FromArrow.SelectedMessage;

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