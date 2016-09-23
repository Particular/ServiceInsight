namespace ServiceInsight.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pirac;

    public class SagaUpdate : BindableObject
    {
        public DateTime FinishTime { get; set; }

        public DateTime StartTime { get; set; }

        public SagaStateChangeStatus Status { get; set; }

        public SagaMessage InitiatingMessage { get; set; }

        public List<SagaTimeoutMessage> OutgoingMessages { get; set; }

        public List<SagaUpdatedValue> Values { get; private set; }

        public bool IsFirstNode => Status == SagaStateChangeStatus.New;

        public bool IsSagaTimeoutMessage => !MissingData && InitiatingMessage.IsSagaTimeoutMessage;

        public List<SagaMessage> NonTimeoutMessages => OutgoingMessages.Where(m => !m.IsTimeout).Cast<SagaMessage>().ToList();

        public List<SagaTimeoutMessage> TimeoutMessages => OutgoingMessages.Where(m => m.IsTimeout).ToList();

        public bool HasNonTimeoutMessages => NonTimeoutMessages.Any();

        public bool HasTimeoutMessages => TimeoutMessages.Any();

        public string StateAfterChange { get; set; }

        public bool MissingData { get; set; }

        void OnStateAfterChangeChanged()
        {
            UpdateValues();
        }

        void OnInitiatingMessageChanged()
        {
            UpdateValues();
        }

        void OnMissingDataChanged()
        {
            UpdateValues();
        }

        void UpdateValues()
        {
            if (string.IsNullOrEmpty(StateAfterChange) || InitiatingMessage == null || MissingData)
            {
                Values = new List<SagaUpdatedValue>();
                return;
            }

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