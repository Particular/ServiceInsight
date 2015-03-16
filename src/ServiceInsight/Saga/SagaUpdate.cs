namespace Particular.ServiceInsight.Desktop.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;

    public class SagaUpdate : PropertyChangedBase
    {
        public DateTime FinishTime { get; set; }

        public DateTime StartTime { get; set; }

        public SagaStateChangeStatus Status { get; set; }

        public SagaMessage InitiatingMessage { get; set; }

        public List<SagaTimeoutMessage> OutgoingMessages { get; set; }

        public List<SagaUpdatedValue> Values { get; private set; }

        public bool IsFirstNode
        {
            get { return Status == SagaStateChangeStatus.New; }
        }

        public bool IsSagaTimeoutMessage
        {
            get { return InitiatingMessage.IsSagaTimeoutMessage; }
        }

        public List<SagaMessage> NonTimeoutMessages
        {
            get { return OutgoingMessages.Where(m => !m.IsTimeout).Cast<SagaMessage>().ToList(); }
        }

        public List<SagaTimeoutMessage> TimeoutMessages
        {
            get { return OutgoingMessages.Where(m => m.IsTimeout).ToList(); }
        }

        public bool HasNonTimeoutMessages { get { return NonTimeoutMessages.Any(); } }

        public bool HasTimeoutMessages { get { return TimeoutMessages.Any(); } }

        public string StateAfterChange { get; set; }

        void OnStateAfterChangeChanged()
        {
            UpdateValues();
        }

        void OnInitiatingMessageChanged()
        {
            UpdateValues();
        }

        void UpdateValues()
        {
            if (string.IsNullOrEmpty(StateAfterChange) || InitiatingMessage == null)
                return;

            Values = JsonPropertiesHelper.ProcessValues(StateAfterChange, s => s.TrimStart('[').TrimEnd(']'))
                                         .Select(v => new SagaUpdatedValue(InitiatingMessage.MessageType, v.Key, v.Value))
                                         .ToList();
        }

        public string Label
        {
            get
            {
                switch (Status)
                {
                    case SagaStateChangeStatus.New:
                        return "Saga Initiated";

                    case SagaStateChangeStatus.Completed:
                    case SagaStateChangeStatus.Updated:
                        return "Saga Updated";
                }

                return string.Empty;
            }
        }
    }

    public enum SagaStateChangeStatus
    {
        Nothing,
        New,
        Updated,
        Completed
    }
}